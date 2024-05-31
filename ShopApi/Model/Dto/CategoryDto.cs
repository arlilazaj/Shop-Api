using System.ComponentModel.DataAnnotations;

namespace ShopApi.Model.Dto;

public class CategoryDto
{
    
    public int Id { get; set; }
    [Required]
    public string Type { get; set; }
    public IList<ProductCopy> Products { get; set; }
}
