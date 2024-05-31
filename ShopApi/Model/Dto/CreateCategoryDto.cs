using System.ComponentModel.DataAnnotations;

namespace ShopApi.Model.Dto;

public class CreateCategoryDto
{
  
  
    [Required]
    public string Type { get; set; }

   
}