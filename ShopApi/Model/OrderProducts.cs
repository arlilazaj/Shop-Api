using System.ComponentModel.DataAnnotations;

namespace ShopApi.Model;

public class OrderProducts
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    public float Total { get; set; }
    public int Quantity { get; set; }
    public Order Order { get; set; }
    public Product Product { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }


}