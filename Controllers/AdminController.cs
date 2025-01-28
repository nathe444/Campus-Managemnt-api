using CampusManagementSystem.Models;
using CampusManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CampusManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMongoCollection<User> _users;

    public AdminController(IAuthService authService, MongoDBContext dbContext)
    {
        _authService = authService;
        _users = dbContext.Users;
    }

    [HttpPost("register/admin")]
    [AllowAnonymous] 
    public async Task<ActionResult<User>> RegisterFirstAdmin([FromBody] RegisterRequest request)
    {
        try
        {
            var adminExists = await _users.Find(u => u.Role == "Admin").AnyAsync();
            if (adminExists)
            {
                return StatusCode(403, new { message = "Admin already exists. New admins can only be created by existing admins." });
            }

            var user = await _authService.RegisterAdminAsync(request);
            return Ok(new { message = "Admin registered successfully", user });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while registering the admin" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register/instructor")]
    public async Task<ActionResult<User>> RegisterInstructor([FromBody] InstructorRegistrationRequest request)
    {
        try
        {
            var user = await _authService.RegisterInstructorAsync(request);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while registering the instructor" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users/{userId}/approve")]
    public async Task<ActionResult<User>> ApproveUser(string userId)
    {
        try
        {
            var user = await _authService.ApproveUserAsync(userId);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while approving the user" });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users/{userId}/deactivate")]
    public async Task<ActionResult> DeactivateUser(string userId)
    {
        try
        {
            var result = await _authService.DeactivateUserAsync(userId);
            if (result)
            {
                return Ok(new { message = "User deactivated successfully" });
            }
            return NotFound(new { message = "User not found" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while deactivating the user" });
        }
    }
}