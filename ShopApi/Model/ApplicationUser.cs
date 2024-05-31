using Microsoft.AspNetCore.Identity;

namespace ShopApi.Model;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; }
    public ICollection<Order> Orders { get; set; }
    public Wishlist Wishlist { get; set; }
}