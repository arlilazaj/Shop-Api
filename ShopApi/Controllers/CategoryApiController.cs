using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Model;
using ShopApi.Model.Dto;
using ShopApi.Repository.IRepository;

namespace ShopApi.Controllers;
[Route("/api/CategoryApi")]
[ApiController]
public class CategoryApiController:ControllerBase
{
    private readonly ICategoryRepository _db ;
    private readonly IMapper _mapper;
    private readonly ApiResponse _response;
    public CategoryApiController(ICategoryRepository dbContext,IMapper mapper)
    {
        _db = dbContext;
        _mapper = mapper;
        this._response = new();
    }


    [HttpGet("getCategory")]
    public async Task<ActionResult<ApiResponse>> GetAllCategory()
    {
        try
        {
            IEnumerable<Category> categoryList = await _db.GetAllAsync(includeProperties:"ProductCategories.Product");
            _response.Result = _mapper.Map<List<CategoryDto>>(categoryList);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>{e.ToString()};
        
        }

        return _response;
    }

    
    [HttpGet("{id:int}", Name = "GetCategoryById")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> GetById(int id)
    {
        try
        {
        if (id==0)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;           
            return BadRequest(_response);
        }
    
        var category = await _db.GetAsync(u => u.Id == id,includeProperties:"ProductCategories.Product");
        if (category==null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            return NotFound(_response);
    
        }
    
        _response.Result = _mapper.Map<CategoryDto>(category);
        _response.IsSuccess = true;
        return Ok(_response);
    
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }
    
        return _response;
    }
    
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> CreateCategory([FromBody]CreateCategoryDto createCategory)
    {

        try
        {
            if (await _db.GetAsync(p => p.Type.ToLower() == createCategory.Type.ToLower()) != null)
            {
                ModelState.AddModelError("ErrorMessage","Product already exists");
                return BadRequest(ModelState);
            }

        
            
        
            if (createCategory==null)
            {
                return BadRequest(createCategory);
            }
        
            Category category = _mapper.Map<Category>(createCategory);
            await _db.CreateAsync(category);

           

            _response.Result = _mapper.Map<CategoryDto>(category);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetCategoryById", new { id = category.Id }, _response);

        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }
    
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpDelete("{id:int}", Name = "DeleteCategory")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> DeleteCategory(int id)
    {
        try
        {
    
        if (id==0)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            BadRequest(_response);
        }
    
        var category = await _db.GetAsync(u => u.Id == id,includeProperties: "ProductCategories.Product");
        if (category==null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            NotFound(_response);
        }
        category.ProductCategories.Clear();
        await _db.DeleteAsync(category);
        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess = true;
        return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }
    
        return _response;
    }
    
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "admin")]
    [HttpPut("{id:int}", Name = "UpdateCategory")]
    public async Task<ActionResult<ApiResponse>> UpdateCategory(int id,[FromBody]UpdateCategoryDto updateCategory)
    {
        if (UpdateCategory==null || updateCategory.Id!=id)
        { 
            _response.StatusCode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }
    
        Category category = _mapper.Map<Category>(updateCategory);
        await _db.UpdateAsync(category);
        _response.IsSuccess = true;
        _response.StatusCode = HttpStatusCode.NoContent;
        return Ok(_response);
    }
}