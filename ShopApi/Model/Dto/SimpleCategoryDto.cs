using System.ComponentModel.DataAnnotations;

namespace ShopApi.Model.Dto;

public class SimpleCategoryDto
{
    public int Id { get; set; }
    [Required]
    public string Type { get; set; }
}