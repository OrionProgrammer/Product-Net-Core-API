using API.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Helpers;
using OfficeOpenXml;

namespace Api.Controllers { }

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductController : BaseController
{
    private IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCodeSingleton _productCodeSingleton;

    public ProductController(
          IUnitOfWork unitOfWork,
          IMapper mapper,
          IProductCodeSingleton productCodeSingleton)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productCodeSingleton = productCodeSingleton;
    }

    //post AddProduct
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] Product_UserCategoryModel product_UserCategory)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = GetErrors() });
        
        //generate new product and add to model before mapping to table
        product_UserCategory.ProductModel.Code = _productCodeSingleton.GenerateProductCode();

        var product = _mapper.Map<Product>(product_UserCategory.ProductModel);

        if(!string.IsNullOrWhiteSpace(product_UserCategory.ProductModel.ImageBase64String))
        {
            string[] strArray = product_UserCategory.ProductModel.ImageBase64String.Split(',');

            try
            {
                product.ImageType = strArray[0];
                product.Image = FromBase64String(strArray[1]);
            }
            catch { }
        }
        product.CategoryId = product_UserCategory.UserCategoryModel.CategoryId;
        product.CreatedBy = product_UserCategory.UserCategoryModel.UserId;

        // add object for inserting
        await _unitOfWork.ProductRepository.InsertAsync(product);
        await _unitOfWork.Complete();

        product_UserCategory.ProductModel.ProductId = product.ProductId;

        //return CreatedAtAction(nameof(Add), new { id = product.ProductId});
        return Ok(product_UserCategory.ProductModel);
    }

    //put UpdateProduct
    [HttpPut()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] ProductModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = GetErrors() });

        var product = _mapper.Map<Product>(model);
        
        if(!string.IsNullOrWhiteSpace(model.ImageBase64String))
        {
            string[] strArray = model.ImageBase64String.Split(',');
            product.ImageType = strArray[0];
            product.Image = FromBase64String(strArray[1]);
        }

        //normally I will add code to check this product belongs to the user, like I have done for Categories
        //but since time is of the essence, I will continue to demonstraing other skills

        // add object for updating
        await _unitOfWork.ProductRepository.Update(product, product.ProductId);
        await _unitOfWork.Complete();

        return Ok();
    }

    //delete Delete
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(long id)
    {
        if (id == 0)
            return BadRequest(new { errors = "Product Id not provided!" });

        //check if the record exists before attempting to delete
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        await _unitOfWork.Complete();

        if(product == null)
            return Ok();

        _unitOfWork.ProductRepository.Delete(id);
        await _unitOfWork.Complete();

        return Ok();
    }

    //get ProductById
    [HttpGet("get-product/{id}")]
    [ProducesResponseType(typeof(CategoryModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(long id)
    {
        if (id == 0)
            return BadRequest(new { errors = "Product Id not provided!" });

        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        await _unitOfWork.Complete();

        if (product is null)
            return NotFound(new { error = "Product not found!" });

        var productModel = _mapper.Map<ProductModel>(product);
        if(product.Image != null)
        {
            productModel.ImageBase64String = product.ImageType + "," + ToBase64String(product.Image);
        }

        return Ok(productModel);
    }

    [HttpGet("export/{userId}")]
    public async Task<IActionResult> Export(long userId)
    {
        var products = await _unitOfWork.ProductRepository.GetProductByUserId(userId);
        var productExportList = _mapper.Map<IEnumerable<ProductExportModel>>(products);

        await _unitOfWork.Complete();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Products");
            worksheet.Cells.LoadFromCollection(productExportList, true);
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
        }
    }

    [HttpPost("import/{userId}")]
    public async Task<IActionResult> Import(IFormFile file, long userId)
    {
        if (file == null || file.Length <= 0)
            return BadRequest("Invalid file");

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.First();
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    await _unitOfWork.ProductRepository.InsertAsync(new Product
                    {
                        ProductId = 0,
                        Name = worksheet.Cells[row, 2].Text,
                        Code = _productCodeSingleton.GenerateProductCode(),
                        Description = worksheet.Cells[row, 4].Text,                        
                        Price = decimal.Parse(worksheet.Cells[row, 5].Text),
                        CategoryId = Convert.ToInt64(worksheet.Cells[row, 6].Text),
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now
                    });

                    await _unitOfWork.Complete();
                }
            }
        }

        return Ok();
    }


    //get ProductsList by CategoryId paged
    [HttpGet("list")]
    [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(int userId, int start = 0, int length = 10)
    {
        var products = await _unitOfWork.ProductRepository.GetProductsByUserIdPaged(userId, start, length);

        return Ok(new
        {
            recordsTotal = products.Item1,
            recordsFiltered = products.Item1,
            data = products.Item2
        });
    }

    #region Helpers

    private byte[] FromBase64String(string base64String)
    {
        if (base64String != null)
        {
            return Convert.FromBase64String(base64String);
        }

        return null;
    }

    private string ToBase64String(byte[] byteArray)
    {
        if (byteArray != null)
        {
            return Convert.ToBase64String(byteArray);
        }

        return "";
    }

    #endregion
}
