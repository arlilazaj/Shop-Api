namespace ShopApi.Model.Dto;

public class ApplicationUserDto
{
    public string Name { get; set; }
    public Guid  Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
   public ICollection<OrderDto> Orders { get; set; }
}