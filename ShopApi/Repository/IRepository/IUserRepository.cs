using ShopApi.Model;
using ShopApi.Model.Dto;

namespace ShopApi.Repository.IRepository;

public interface IUserRepository:IRepository<ApplicationUser>
{
    
    bool isUnique(string username);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
    Task<ApplicationUser> UpdateAsync(ApplicationUser entity);    
}