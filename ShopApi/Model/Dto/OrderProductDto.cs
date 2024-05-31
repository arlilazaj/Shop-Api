using System.ComponentModel.DataAnnotations;

namespace ShopApi.Model.Dto;

public class OrderProductDto
{
    public int Quantity { get; set; }
     public int ProductId { get; set; }
    public int OrderId { get; set; }
    public float Price { get; set; } 
    public float Total { get; set; } 
}