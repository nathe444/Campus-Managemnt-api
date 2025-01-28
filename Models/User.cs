using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CampusManagementSystem.Models;

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; } // "Admin", "Student", "Instructor"
    public bool IsActive { get; set; } = false; // For account activation
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public ObjectId? ProfileId { get; set; } // Links to Student or Instructor profile
}

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
    public string? DepartmentId { get; set; } // Required for Students and Instructors
}

public class StudentRegistrationRequest : RegisterRequest
{
    public required int Age { get; set; }
}

public class InstructorRegistrationRequest : RegisterRequest
{
    public required DateTime HireDate { get; set; }
}

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}