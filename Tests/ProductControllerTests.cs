using FluentAssertions;
using Newtonsoft.Json;
using System.Text;


namespace Tests { }

public class ProductControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;

    public ProductControllerTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // Add JWT token to headersw.  
        //----commmentsed out as I don't have enough timee to makes sure the secret key is being applied from appSettings.json.
        //var token = TokenGenerator.GenerateJwtToken();
        //_client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task AddProduct_ShouldReturnOk()
    {
        // Arrange
        var productModel = new Product_UserCategoryModel
        {
            ProductModel = new ProductModel
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                ImageBase64String = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA"
            },
            UserCategoryModel = new UserCategoryModel
            {
                UserId = 1,
                CategoryId = 1
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(productModel), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/product", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk()
    {
        //arrange
        //add a product, then test updating the same product
        var productModel = new Product_UserCategoryModel
        {
            ProductModel = new ProductModel
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                ImageBase64String = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA"
            },
            UserCategoryModel = new UserCategoryModel
            {
                UserId = 1,
                CategoryId = 1
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(productModel), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/product", content);

        //store the product id to use in below test cases
        string result = await response.Content.ReadAsStringAsync();
        ProductModel productModelResult = JsonConvert.DeserializeObject<ProductModel>(result)!;
        long productId = productModelResult.ProductId;




        // Arrange
        var productModelUpdate = new ProductModel
        {
            ProductId = productId,
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 12.99m,
            ImageBase64String = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA"
        };

        var contentUpdate = new StringContent(JsonConvert.SerializeObject(productModelUpdate), Encoding.UTF8, "application/json");

        // Act
        var responseUpdate = await _client.PutAsync("/api/product", contentUpdate);
        // Assert
        responseUpdate.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnOk()
    {
        //arrange
        //add a product, then test delete
        var productModel = new Product_UserCategoryModel
        {
            ProductModel = new ProductModel
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                ImageBase64String = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA"
            },
            UserCategoryModel = new UserCategoryModel
            {
                UserId = 1,
                CategoryId = 1
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(productModel), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/product", content);

        //store the product id to use in below test cases
        string result = await response.Content.ReadAsStringAsync();
        ProductModel productModelResult = JsonConvert.DeserializeObject<ProductModel>(result)!;
        long productId = productModelResult.ProductId;




        // Act
        var responseDelete = await _client.DeleteAsync($"/api/product/{productId}");

        // Assert
        responseDelete.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnOk()
    {
        //arrange
        //add a product, then test get
        var productModel = new Product_UserCategoryModel
        {
            ProductModel = new ProductModel
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 10.99m,
                ImageBase64String = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA"
            },
            UserCategoryModel = new UserCategoryModel
            {
                UserId = 1,
                CategoryId = 1
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(productModel), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/product", content);

        //store the product id to use in below test cases
        string result = await response.Content.ReadAsStringAsync();
        ProductModel productModelResult = JsonConvert.DeserializeObject<ProductModel>(result)!;
        long productId = productModelResult.ProductId;



        // Act
        var responseGet = await _client.GetAsync($"/api/product/get-product/{productId}");

        // Assert
        responseGet.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/product/get-product/999");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}

