using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ShopApi.Model.Dto;

public class ProductDto
{
   
    public int Id { get; set; }
    public string Image { get; set; }
  
    public string Description  { get; set; }
    public string Specification { get; set; }
    public string Reviews { get; set; }
    [Required]
    public float Price  { get; set; }
    [Required]
    public int Rating  { get; set; }
    [Required]
    public string Type  { get; set; }
    public int DisccountPrice  { get; set; }
    public int Stock  { get; set; }
    public string Tag  { get; set; }
    public int UnitSold { get; set; }
    public int Countdown_days { get; set; }
    public int Countdown_hours { get; set; }
    public int Countdown_min { get; set; }
    public int Countdown_sec { get; set; }
    public ICollection<SimpleCategoryDto> Categories { get; set; }
}
