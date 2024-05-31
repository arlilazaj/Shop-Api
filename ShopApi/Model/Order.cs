using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Model;

public class Order
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int  Id { get; set; }
   

    public Guid UserId { get; set; }

 
    public ApplicationUser User { get; set; }
    

      
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<OrderProducts> OrderProducts { get; set; } = new List<OrderProducts>();
}