using System.ComponentModel.DataAnnotations;

namespace ShopApi.Model.Dto;

public class ProductCategoryDto
{
    [Required] public int Product_Id { get; set; }
    [Required] public int Category_Id { get; set; }
    public ProductDto Product { get; set; }
    public CategoryDto Category { get; set; }
}