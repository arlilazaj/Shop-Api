using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Model;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Image { get; set; }
    
    public string Description  { get; set; }
    public string Specification { get; set; }
    public string Reviews { get; set; }
    
    public float Price  { get; set; }
   
    public int  Rating  { get; set; }
    
    public string Type  { get; set; }
    public int DisccountPrice  { get; set; }
    public int Stock  { get; set; }
    public string Tag  { get; set; }
    public int UnitSold { get; set; }
    public int? Countdown_days { get; set; }
    public int? Countdown_hours { get; set; }
    public int? Countdown_min { get; set; }
    public int? Countdown_sec { get; set; }
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    public ICollection<OrderProducts> OrderProducts { get; set; } = new List<OrderProducts>();
    public ICollection<ProductWishlist> ProductWishlists { get; set; } = new List<ProductWishlist>();
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}