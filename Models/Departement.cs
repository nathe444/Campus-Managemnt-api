using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace CampusManagementSystem.Models;

public class Department
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public ObjectId Id { get; set; }
    
    public required string Name { get; set; }
    public required string Code { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? HeadOfDepartment { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public ObjectId? HeadInstructorId { get; set; }
    
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int MaxStudents { get; set; } = 100;

    // Add string property for Id serialization
    [BsonIgnore]
    public string StringId => Id.ToString();
    [BsonIgnore]
    public string? StringHeadInstructorId => HeadInstructorId?.ToString();
}

// DTO for creating a new department
public class CreateDepartmentRequest
{
    public required string Name { get; set; }
    public required string Code { get; set; }
    public string? Description { get; set; }
    public string? HeadOfDepartment { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? MaxStudents { get; set; }
}

// DTO for updating a department
public class UpdateDepartmentRequest
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? HeadOfDepartment { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? MaxStudents { get; set; }
    public bool? IsActive { get; set; }
}
