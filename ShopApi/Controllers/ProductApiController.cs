using System.Linq.Expressions;
using System.Net;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Model;
using ShopApi.Model.Dto;
using ShopApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ShopApi.Controllers;
[Route("api/ProductApi")]
[ApiController]
public class ProductApiController : ControllerBase
{
    private readonly IProductRepository _db ;
    private readonly ApplicationDbContext _dbContext;
    private readonly ICategoryRepository _dbCategory ;
    private readonly IMapper _mapper;
    private readonly ApiResponse _response;
 
    public ProductApiController(IProductRepository db,IMapper mapper,ICategoryRepository dbCategory,ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _db = db;
        _dbCategory = dbCategory;
        _mapper = mapper;
        this._response = new();
    }


    [HttpGet("getProducts")]
    public async Task<ActionResult<ApiResponse>> GetProducts(
        [FromQuery]string? search, int pageSize=0,int pageNumber=1)
    {
        try
        {
            
            IEnumerable<Product> productsList = await _db.GetAllAsync(includeProperties:"ProductCategories.Category",pageSize:pageSize,pageNumber:pageNumber);
            var allProducts = await _db.GetAllAsync();
            int totalCount = allProducts.Count();
            
            
            if (!string.IsNullOrEmpty(search))
            {
                productsList = productsList.Where(u =>  u.Type.ToLower().Contains(search));
            }

            Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };
            Response.Headers.Add("X-Pagination",JsonConvert.SerializeObject(pagination));

            _response.Total = totalCount;
            _response.Result = _mapper.Map<List<ProductDto>>(productsList);
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
    [HttpGet("sortByExpression")]
    public async Task<ActionResult<ApiResponse>> sortByExpression(
        int pageSize=0,int pageNumber=1,string orderBy="")
    {
        try
        {
            Expression<Func<Product, object>> orderByExpression = null;

            if (string.IsNullOrEmpty(orderBy))
            {
                
                orderByExpression = p => p.Id;
            }
            else
            {
                switch (orderBy)
                {
                    case "Type":
                        orderByExpression = p => p.Type;
                        break;
                    case "Price":
                        orderByExpression = p => p.Price;
                        break;
               
                    default:
                   
                        return BadRequest("Invalid orderBy parameter.");
                }
            }
            
            IEnumerable<Product> productsList = await _db.GetAllAsync(includeProperties:"ProductCategories.Category",pageSize:pageSize,pageNumber:pageNumber,orderBy:orderByExpression);
          
            var allProducts = await _db.GetAllAsync();
            int totalCount = allProducts.Count();
            Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };
            Response.Headers.Add("X-Pagination",JsonConvert.SerializeObject(pagination));

            _response.Total = totalCount;
            _response.Result = _mapper.Map<List<ProductDto>>(productsList);
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


    [HttpGet("{id:int}", Name = "GetProductById")]
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

        var product = await _db.GetAsync(u => u.Id == id,includeProperties:"ProductCategories.Category");
        if (product==null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            return NotFound(_response);

        }

        _response.Result = _mapper.Map<ProductDto>(product);
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> CreateProduct([FromBody]CreateProductDto createProduct)
    {

        try
        {
        if (await _db.GetAsync(p => p.Type.ToLower() == createProduct.Type.ToLower()) != null)
        {
            ModelState.AddModelError("ErrorMessage","Product already exists");
            return BadRequest(ModelState);
        }
        byte[] decodedImage = string.IsNullOrEmpty(createProduct.Image) ? null : Convert.FromBase64String(createProduct.Image);
        
        var existingId = await _dbCategory.GetAllAsync(c => createProduct.categoryId.Contains(c.Id));
        if (existingId.Count==0)
        {
            ModelState.AddModelError("ErrorMessages","Category ID is invalid");
               return BadRequest(ModelState);
        }
       
        
        if (createProduct==null)
        {
            return BadRequest(createProduct);
        }
        
        Product product = _mapper.Map<Product>(createProduct);
        await _db.CreateAsync(product);

        foreach (var categorieId in createProduct.categoryId)
        {
        var productCategory = new ProductCategory { Product_Id = product.Id, Category_Id = categorieId};
        product.ProductCategories.Add(productCategory);
            
        }
            
        
        await _db.SaveAsync();

        _response.Result = _mapper.Map<ProductDto>(product);
        _response.StatusCode = HttpStatusCode.Created;
        return CreatedAtRoute("GetProductById", new { id = product.Id }, _response);

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
    [HttpDelete("{id:int}", Name = "DeleteProduct")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> DeleteProduct(int id)
    {
        try
        {

        if (id==0)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            BadRequest(_response);
        }

        var product = await _db.GetAsync(u => u.Id == id);                
        if (product==null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            NotFound(_response);
        }

        await _db.DeleteAsync(product);
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
    [HttpPut("{id:int}", Name = "UpdateProduct")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> UpdateProduct(int id,[FromBody]UpdateProductDto updateProduct)
    {
        try
        {
            if (updateProduct==null || updateProduct.Id!=id)
            { 
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            var existingProduct = await _db.GetAsync(p => p.Id == id,tracked:false ,includeProperties: "ProductCategories");
            if (existingProduct == null)
            {
             
                return NotFound("Product not found");
             }
            if (string.IsNullOrEmpty(updateProduct.Image))
            {
                
                updateProduct.Image = existingProduct.Image; 
            }
            byte[] decodedImage = string.IsNullOrEmpty(updateProduct.Image) ? null : Convert.FromBase64String(updateProduct.Image);
            var existingId = await _dbCategory.GetAllAsync(c => updateProduct.categoryId.Contains(c.Id));
            if (existingId.Count==0)
            {
                ModelState.AddModelError("ErrorMessages","Category ID is invalid");
                return BadRequest(ModelState);
            }
     
            foreach (var existingCategory in existingProduct.ProductCategories)
            {
                _dbContext.Entry(existingCategory).State = EntityState.Detached;
            }

            _dbContext.ProductCategories.RemoveRange(existingProduct.ProductCategories);
      

            await _db.SaveAsync();
            _mapper.Map(updateProduct, existingProduct);

            foreach (var categoryId in updateProduct.categoryId)
            {
                var productCategory = new ProductCategory { Product_Id = existingProduct.Id, Category_Id = categoryId };
                existingProduct.ProductCategories.Add(productCategory);
            }


            await _db.UpdateAsync(existingProduct);
    
       
        
        
            _response.IsSuccess = true;
        
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }
}