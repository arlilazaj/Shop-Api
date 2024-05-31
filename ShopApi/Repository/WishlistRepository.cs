using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Model;
using ShopApi.Model.Dto;
using ShopApi.Repository.IRepository;

namespace ShopApi.Repository;

public class WishlistRepository:Repository<Wishlist>,IWishlistRepository
{
    private readonly ApplicationDbContext _db;
    private readonly ApiResponse _response;
    public WishlistRepository(ApplicationDbContext db):base(db)
    {
        this._response = new ApiResponse();
        _db = db;
    }
    

    public async Task AddProductToWishlist(CreateProductWishlist createProductWishlist)
    {
        var existWishlist = await _db.Wishlist.FindAsync(createProductWishlist.WishlistId);
        var existingProduct = await _db.Products.FindAsync(createProductWishlist.ProductIds);
        if (existWishlist==null || existingProduct==null)
        {
            _response.ErrorMessages = new List<string>() { "Wishlist not found or product not found" };
        }

        if (!_db.ProductWishlists.Any(p=>p.ProductId==createProductWishlist.ProductIds  && p.WishlistId==createProductWishlist.WishlistId))
            {
                var productWishlist = new ProductWishlist
                {
                    ProductId = createProductWishlist.ProductIds,
                    WishlistId = createProductWishlist.WishlistId
                };
                _db.ProductWishlists.Add(productWishlist);
            }
        

        await _db.SaveChangesAsync();
    }
    
}