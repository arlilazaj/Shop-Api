using System.ComponentModel.DataAnnotations;

namespace ShopApi.Model;

public class ProductCategory
{
     public int Product_Id { get; set; }
     public int Category_Id { get; set; }

     public Product Product { get; set; }
     public Category Category { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
