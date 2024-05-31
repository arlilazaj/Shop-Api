using System.ComponentModel.DataAnnotations;
namespace ShopApi.Model.Dto;

public class loginDTO
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
}