using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Model;

public class Wishlist
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int WishlistId { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
    public ICollection<ProductWishlist> ProductWishlists { get; set; } = new List<ProductWishlist>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}