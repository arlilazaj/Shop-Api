namespace ShopApi.Model.Dto;

public class CreateOrderDto
{
 



   
    public Guid UserId { get; set; }
  
    public IList<int> ProductId { get; set; }
    public IList<int> Quantity { get; set; }
}