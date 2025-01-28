using CampusManagementSystem.Models;

namespace CampusManagementSystem.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<User> RegisterStudentAsync(StudentRegistrationRequest request);
    Task<User> RegisterInstructorAsync(InstructorRegistrationRequest request);
    Task<User> RegisterAdminAsync(RegisterRequest request);
    Task<User> ApproveUserAsync(string userId);
    Task<bool> DeactivateUserAsync(string userId);
    string GenerateJwtToken(User user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}