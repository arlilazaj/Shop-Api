namespace ShopApi.Model.Dto;

public class WishlistDto
{
    public int WishlistId { get; set; }
    public ICollection<ProductDto> Products { get; set; }
}