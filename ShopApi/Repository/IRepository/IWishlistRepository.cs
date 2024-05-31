using ShopApi.Model;
using ShopApi.Model.Dto;

namespace ShopApi.Repository.IRepository;

public interface IWishlistRepository:IRepository<Wishlist>
{
     Task AddProductToWishlist(CreateProductWishlist createProductWishlist);
}