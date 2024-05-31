using System.ComponentModel.DataAnnotations;

namespace ShopApi.Model.Dto;

public class OrderDto
{
   public int  Id { get; set; }
 

    public float Total { get; set; }
    public string OrderDetails  { get; set; }
    public Guid UserId { get; set; }
    
    public IList<OrderProductDto> Product { get; set; }      

}