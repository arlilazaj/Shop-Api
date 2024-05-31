namespace ShopApi.Model.Dto;

public class UpdateOrderDto
{
    public int Id { get; set; }



   
    public int UserId { get; set; }
    public string OrderDetails  { get; set; }
    public IList<int> ProductId { get; set; }
    public IList<int> Quantity { get; set; }
}