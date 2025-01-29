using CampusManagementSystem.Models;
using CampusManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while processing your request" });
        }
    }

    [HttpPost("register/student")]
    public async Task<ActionResult<User>> RegisterStudent([FromBody] StudentRegistrationRequest request)
    {
        try
        {
            var user = await _authService.RegisterStudentAsync(request);
            return Ok(new { 
                message = "Registration successful. Please wait for admin approval.",
                user = new
                {
                    id = user.StringId,
                    user.Email,
                    user.Role,
                    user.IsActive,
                    user.CreatedAt,
                    user.LastLogin,
                    profileId = user.StringProfileId
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while registering" });
        }
    }
}