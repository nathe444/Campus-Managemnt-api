using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace CampusManagementSystem.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public ObjectId Id { get; set; }
    
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; } // "Admin", "Student", "Instructor"
    public bool IsActive { get; set; } = false; // For account activation
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public ObjectId? ProfileId { get; set; } // Links to Student or Instructor profile

    // Add string properties for Id serialization
    [BsonIgnore]
    public string StringId => Id.ToString();
    
    [BsonIgnore]
    public string? StringProfileId => ProfileId?.ToString();
}

// Base request for admin registration
public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
}

// Student registration with required department and age
public class StudentRegistrationRequest : RegisterRequest
{
    public required int Age { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public required string DepartmentId { get; set; }
}

// Instructor registration with required department and hire date
public class InstructorRegistrationRequest : RegisterRequest
{
    public required DateTime HireDate { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public required string DepartmentId { get; set; }
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