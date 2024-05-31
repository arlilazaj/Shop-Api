using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Model;

public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Type { get; set; }

    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}