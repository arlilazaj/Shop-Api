namespace ShopApi.Model;

public class ProductWishlist
{
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int WishlistId { get; set; }
    public Wishlist Wishlist { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}