using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Model;
using ShopApi.Model.Dto;
using ShopApi.Repository.IRepository;

namespace ShopApi.Controllers;

[Route("api/WishlistApi")]
[ApiController]



public class ProductWishlistApiController:ControllerBase
{
    private readonly IWishlistRepository _db;
    private readonly IMapper _mapper;
    private ApiResponse _response { get; set; }

    public ProductWishlistApiController(IWishlistRepository db,IMapper mapper)
    {
        _mapper = mapper;
        _db = db;
        this._response = new ApiResponse(); 
    }

    
    [HttpGet("getAllWishlists")]
    public async Task<ActionResult<ApiResponse>> GetProducts()
    {
        try
        {
            IEnumerable<Wishlist> wishlists = await _db.GetAllAsync(includeProperties:"ProductWishlists.Product");
            _response.Result = _mapper.Map<List<WishlistDto>>(wishlists);
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
    [HttpGet("{wishlistId:int}", Name = "GetWishlistById")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> GetById(int wishlistId)
    {
        try
        {
            if (wishlistId==0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;           
                return BadRequest(_response);
            }

            var wishlist = await _db.GetAsync(u => u.WishlistId == wishlistId,includeProperties:"ProductWishlists.Product");
            if (wishlistId==null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);

            }

            _response.Result = _mapper.Map<WishlistDto>(wishlist);
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
    
    [HttpPost("addProductToWishlist")]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<ApiResponse>> AddProductToWishlist([FromBody]CreateProductWishlist createProductWishlist)
    
    {
        try
        {
              await _db.AddProductToWishlist(createProductWishlist);

              _response.ErrorMessages.Add("product added to the wishlist");
              return Ok(_response);

        }
        catch (Exception e)
        {
            
            _response.IsSuccess=false;
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages.Add("ProductId or WishlistId is invalid");
        }

        return _response;
    }

    [HttpDelete("removeProduct/{wishlistId}/{productId}")]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<ApiResponse>> DeleteProductFromWishlist(int wishlistId,int productId)
    {
        try
        {
        Wishlist wishlist =await _db.GetAsync(w => w.WishlistId == wishlistId, includeProperties: "ProductWishlists.Product");
        if (wishlist==null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            NotFound(_response);
        }

        ProductWishlist productWishlist = wishlist.ProductWishlists.FirstOrDefault(p => p.ProductId == productId);
        if (productWishlist!=null)
        {
            wishlist.ProductWishlists.Remove(productWishlist);
            await _db.SaveAsync();
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        else
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            NotFound(_response);
        }

        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
          
            
        }
        return _response;
    } 
}