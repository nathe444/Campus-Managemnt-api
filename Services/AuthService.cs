using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CampusManagementSystem.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CampusManagementSystem.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<Student> _students;
    private readonly IMongoCollection<Instructor> _instructors;

    public AuthService(IConfiguration configuration, MongoDBContext dbContext)
    {
        _configuration = configuration;
        _users = dbContext.Users;
        _students = dbContext.Students;
        _instructors = dbContext.Instructors;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
        
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is not activated. Please contact administrator.");
        }

        // Update last login
        user.LastLogin = DateTime.UtcNow;
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);

        return new LoginResponse
        {
            Token = GenerateJwtToken(user),
            Role = user.Role,
            Email = user.Email,
            IsActive = user.IsActive
        };
    }

    public async Task<User> RegisterStudentAsync(StudentRegistrationRequest request)
    {
        // Check if email already exists
        if (await _users.Find(u => u.Email == request.Email).AnyAsync())
        {
            throw new InvalidOperationException("Email already registered");
        }

        // Create student profile
        var student = new Student
        {
            Name = request.Name,
            Email = request.Email,
            Age = request.Age,
            EnrollmentDate = DateTime.UtcNow,
            DepartmentId = ObjectId.Parse(request.DepartmentId!)
        };
        await _students.InsertOneAsync(student);

        // Create user account
        var user = new User
        {
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Role = "Student",
            IsActive = false, // Requires admin approval
            ProfileId = student.Id
        };
        await _users.InsertOneAsync(user);

        return user;
    }

    public async Task<User> RegisterInstructorAsync(InstructorRegistrationRequest request)
    {
        // Check if email already exists
        if (await _users.Find(u => u.Email == request.Email).AnyAsync())
        {
            throw new InvalidOperationException("Email already registered");
        }

        // Create instructor profile
        var instructor = new Instructor
        {
            Name = request.Name,
            Email = request.Email,
            HireDate = request.HireDate,
            DepartmentId = ObjectId.Parse(request.DepartmentId!)
        };
        await _instructors.InsertOneAsync(instructor);

        // Create user account
        var user = new User
        {
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Role = "Instructor",
            IsActive = true, // Instructors are automatically activated
            ProfileId = instructor.Id
        };
        await _users.InsertOneAsync(user);

        return user;
    }

    public async Task<User> RegisterAdminAsync(RegisterRequest request)
    {
        // Check if email already exists
        if (await _users.Find(u => u.Email == request.Email).AnyAsync())
        {
            throw new InvalidOperationException("Email already registered");
        }

        var user = new User
        {
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Role = "Admin",
            IsActive = true // Admin accounts are automatically activated
        };
        await _users.InsertOneAsync(user);

        return user;
    }

    public async Task<User> ApproveUserAsync(string userId)
    {
        var id = ObjectId.Parse(userId);
        var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        user.IsActive = true;
        await _users.ReplaceOneAsync(u => u.Id == id, user);
        
        return user;
    }

    public async Task<bool> DeactivateUserAsync(string userId)
    {
        var id = ObjectId.Parse(userId);
        var result = await _users.UpdateOneAsync(
            u => u.Id == id,
            Builders<User>.Update.Set(u => u.IsActive, false)
        );
        
        return result.ModifiedCount > 0;
    }

    public string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("ProfileId", user.ProfileId?.ToString() ?? string.Empty)
            }),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryInMinutes"]!)),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}