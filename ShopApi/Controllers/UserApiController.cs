using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Model;
using ShopApi.Model.Dto;
using ShopApi.Repository.IRepository;

namespace ShopApi.Controllers;
[Route("api/User")]
[ApiController]
public class UserApiController:ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly ApiResponse _response;
    private readonly UserManager<ApplicationUser> _userManager;
    private IMapper _mapper;

    public UserApiController(IUserRepository userRepository, UserManager<ApplicationUser> userManager,IMapper mapper)
    {
        _mapper = mapper;
        _userManager = userManager;
        _userRepo = userRepository;
        this._response = new ApiResponse();
    }


    [HttpGet("getUsers")]
    public async Task<ActionResult<ApiResponse>> GetUsers()
    {
        try
        {
            IEnumerable<ApplicationUser> userList = await _userRepo.GetAllAsync(includeProperties:"Orders");
            _response.Result = _mapper.Map<List<ApplicationUserDto>>(userList);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>{e.ToString()};
        }

        return _response;
    }
    
    
  
    [HttpDelete("deleteUser/{id:guid}", Name = "DeleteUser")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> DeleteProduct(Guid id)
    {
        try
        {

            if (id==Guid.Empty)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                BadRequest(_response);
            }

            var user = await _userRepo.GetAsync(u => u.Id == id);                
            if (user==null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                NotFound(_response);
            }

            await _userRepo.DeleteAsync(user);
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }
    
    [Authorize(Roles = "admin")]
    [HttpPut("UpdateUser/{id}",Name = "UpdateUser")]
    public async Task<ActionResult<ApiResponse>> UpdateUser(Guid id,
        [FromBody] UpdateUserDto userUpdate)
    {
        try
        {
            if (id==Guid.Empty)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                BadRequest(_response);
            }
            

            var user = await _userRepo.GetAsync(user => user.Id == id);
            if (user==null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                NotFound(_response);
            }

            user.Name = userUpdate.Name;
            user.Email = userUpdate.Email;
            user.UserName = userUpdate.Username;
            
         
            await _userRepo.UpdateAsync(user);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }
        return _response;
    }

    [Authorize(Roles = "admin")]
    [HttpPut("UpdateUserPassword/{id}", Name = "UpdateUserPassword")]
    public async Task<ActionResult<ApiResponse>> UpdateUser(Guid id,
        [FromBody] UpdatePasswordDto userUpdate)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                BadRequest(_response);
            }

            var user = await _userRepo.GetAsync(user => user.Id == id);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                NotFound(_response);
            }



            if (!string.IsNullOrEmpty(userUpdate.Password))
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {

                    return BadRequest(removePasswordResult.Errors.Select(e => e.Description));
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, userUpdate.Password);
                if (!addPasswordResult.Succeeded)
                {

                    return BadRequest(addPasswordResult.Errors.Select(e => e.Description));
                }
            }


            await _userRepo.UpdateAsync(user);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var loginResponse = await _userRepo.Login(loginRequestDto);
        if (loginResponse.User==null || string.IsNullOrEmpty(loginResponse.Token))
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username or password is incorrect");
            return BadRequest(_response);
        }
        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.ErrorMessages.Add("You are logged in");
        _response.Result = loginResponse;
        return Ok(_response);
    }


    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
    {
        bool isUsernameUnique =  _userRepo.isUnique(registrationRequestDto.Username);
        if (isUsernameUnique==false)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username already exists");
            return BadRequest(_response);
        }

        var registerUser = await _userRepo.Register(registrationRequestDto);
        if (registerUser==null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Error while registering");
            return BadRequest(_response);
        }
        else
        {
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = registerUser;
            return Ok(_response);
        }
    }
}