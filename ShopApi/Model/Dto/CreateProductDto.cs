using System.ComponentModel.DataAnnotations;

namespace ShopApi.Model.Dto;

public class CreateProductDto
{
    
    public string Image { get; set; }
    
    public string Description  { get; set; }
    public string Specification { get; set; }
    public string Reviews { get; set; }
   
    public float Price  { get; set; }
   
    public int? Rating  { get; set; }
   
    public string Type  { get; set; }
    public int DisccountPrice  { get; set; }
    public int Stock  { get; set; }
    public string Tag  { get; set; }
    public int? Countdown_days { get; set; }
    public int? Countdown_hours { get; set; }
    public int? Countdown_min { get; set; }
    public int? Countdown_sec { get; set; }
    [Required] public IList<int> categoryId { get; set; }   
   
 
}