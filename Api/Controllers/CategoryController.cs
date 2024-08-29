using API.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Helpers;
using System.Runtime.CompilerServices;

namespace Api.Controllers { }

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoryController : BaseController
{
    private IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(
          IUnitOfWork unitOfWork,
          IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    //post AddCategory
    [HttpPost("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] CategoryModel model, long userId)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = GetErrors() });

        var cat = _mapper.Map<Category>(model);
        cat.CreatedBy = userId;

        // add object for inserting
        await _unitOfWork.CategoryRepository.InsertAsync(cat);
        await _unitOfWork.Complete();


        //add relationship with user table
        UserCategory uc = new()
        {
            UserId = userId,
            CategoryId = cat.CategoryId
        };

        await _unitOfWork.UserCategoryRepository.InsertAsync(uc);
        await _unitOfWork.Complete();

        model.CategoryId = cat.CategoryId;

        //abandon CreatedAction as it's not returning the model to bind on UI side :/
        //return CreatedAtAction(nameof(Add), new { model });

        return Ok(model);
    }

    //put UpdateCategory
    [HttpPut("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] CategoryModel model, long userId)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = GetErrors() });

        var cat = _mapper.Map<Category>(model);

        //check if category belongs to user
        if(!await UserOwnsCategory(model.CategoryId, userId))
            return Unauthorized(nameof(Update)); //return unauthorized as the user is not a super admin to modify the category

        // add object for updating
        await _unitOfWork.CategoryRepository.Update(cat, cat.CategoryId);
        await _unitOfWork.Complete();

        return Ok();
    }

    //delete Delete
    [HttpDelete()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromBody]UserCategoryModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = GetErrors() });

        //check if category belongs to user
        if (!await UserOwnsCategory(model.CategoryId, model.UserId))
            return Unauthorized(nameof(Update)); //return unauthorized as the user is not a super admin to modify the category

        _unitOfWork.CategoryRepository.Delete(model.CategoryId);

        await _unitOfWork.Complete();

        return Ok();
    }

    //get CategoryById and UserId CategoryId
    [HttpGet("get-category/{userId}/{categoryId}")]
    [ProducesResponseType(typeof(CategoryModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(long userId, long categoryId)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = GetErrors() });

        var cat = await _unitOfWork.CategoryRepository.GetCategoryByUserIdCategoryId(userId, categoryId);
        await _unitOfWork.Complete();

        if (cat is null)
            return NotFound(new { error = "Category not found!" });

        return Ok(cat);
    }

    //get all by userid
    [HttpGet("get-all/{userId}")]
    [ProducesResponseType(typeof(CategoryModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(long userId)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = GetErrors() });

        var list = await _unitOfWork.CategoryRepository.GetCategoryListByUserId(userId);
        await _unitOfWork.Complete();

        if (list is null)
            return NotFound(new { error = "No records found!" });

        return Ok(list);
    }

    //get paged CategoryList by UserId
    [HttpGet("list")]
    [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]
    public async Task<List<Category>> List([FromBody] PagingModel model)
    {
        return await _unitOfWork.CategoryRepository.GetCategoriesByUserIdPaged(model.Id, model.PageNo, model.PageSize);
    }

    //applying inlining as this method will be called numerous times and I would like to speed up performance
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task<bool> UserOwnsCategory(long categoryId, long userId)
    {
        bool exists = await _unitOfWork.UserCategoryRepository.CheckIfUserCategoryExists(categoryId, userId);
        await _unitOfWork.Complete();
        return exists;
    }


    [HttpGet("test")]
    [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]
    public async Task<List<Category>> Test()
    {
        return (List<Category>)await _unitOfWork.CategoryRepository.GetAllAsync();
    }
}