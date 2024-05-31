using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Data;
using ShopApi.Model;
using ShopApi.Model.Dto;
using ShopApi.Repository.IRepository;

namespace ShopApi.Repository;

public class UserRepository:Repository<ApplicationUser>,IUserRepository
{
   
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private string secretKey;
    private readonly IMapper _mapper;
    public UserRepository(ApplicationDbContext db,IConfiguration configuration,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager,IMapper mapper):base(db)
    {
      
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        this.secretKey = configuration.GetValue<string>("ApiSettings:Secret");
    }
    public bool isUnique(string username)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.Trim().ToLower() == username.Trim().ToLower());
        if (user!=null)
        {
            return false;
        }

        return true;
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        var user = _db.ApplicationUsers
            .Include(u => u.Wishlist)
            .FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.Username.ToLower());
        bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

        if (user==null || isValid==false)
        {
            return new LoginResponseDto()
            {
                Token = "",
                User = null
            };
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var roles = await _userManager.GetRolesAsync(user);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault())
            }),
            Expires = DateTime.UtcNow.AddDays(200),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
 
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var userDto = _mapper.Map<UserDto>(user);
        if (user.Wishlist != null) userDto.WishlistId = user.Wishlist.WishlistId;
        LoginResponseDto loginResponseDto = new LoginResponseDto()
        {
            Token = tokenHandler.WriteToken(token),
            User =userDto
            
         
        };
        return loginResponseDto;
    }

    public async Task<UserDto> Register(RegistrationRequestDto registrationRequestDto)
    {
        if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>("admin"));
            await _roleManager.CreateAsync(new IdentityRole<Guid>("customer"));
        }

    
        ApplicationUser adminUser = new()
        {
            UserName = "admin",
            Email = "admin@gmail.com",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            Name = "admin"
        };
        var adminUserExists = await _userManager.FindByNameAsync("admin");
        if (adminUserExists==null)
        {
            var adminResult = await _userManager.CreateAsync(adminUser, "Admin123@");
            if (adminResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "admin");
            }
           
        }
        ApplicationUser user = new()
        {
            UserName = registrationRequestDto.Username,
            Email = registrationRequestDto.Email,
            NormalizedEmail = registrationRequestDto.Email.ToUpper(),
            Name = registrationRequestDto.Name
        };
        var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);

        if (result.Succeeded)
        {

            Wishlist wishlist = new Wishlist()
            {
                UserId = user.Id,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.Wishlist.AddAsync(wishlist);
            await _db.SaveChangesAsync();
            await _userManager.AddToRoleAsync(user, "customer");
            var userDto = _mapper.Map<UserDto>(user);
            userDto.WishlistId = wishlist.WishlistId; 

            return userDto;

        }

        return new UserDto();
    }

    public async Task<ApplicationUser> UpdateAsync(ApplicationUser entity)
    {
        
        _db.ApplicationUsers.Update(entity);
        await SaveAsync();
        return entity;
    }

    
}