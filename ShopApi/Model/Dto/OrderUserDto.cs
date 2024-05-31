namespace ShopApi.Model.Dto;

public class OrderUserDto
{
    public int  Id { get; set; }
 

    public float Total { get; set; }
    public string OrderDetails  { get; set; }
    public Guid UserId { get; set; }
}