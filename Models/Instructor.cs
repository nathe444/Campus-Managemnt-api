using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace CampusManagementSystem.Models;

public class Instructor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public ObjectId Id { get; set; }

    public required string Name { get; set; }
    public required string Email { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public ObjectId DepartmentId { get; set; }
    
    public required DateTime HireDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties for serialization
    [BsonIgnore]
    public string StringId => Id.ToString();
    
    [BsonIgnore]
    public string StringDepartmentId => DepartmentId.ToString();
}

public class CreateInstructorRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string DepartmentId { get; set; }
    public required DateTime HireDate { get; set; }
}

public class UpdateInstructorRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? DepartmentId { get; set; }
    public DateTime? HireDate { get; set; }
    public bool? IsActive { get; set; }
}