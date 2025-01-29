using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace CampusManagementSystem.Models;

public class Student
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public ObjectId Id { get; set; }
    
    public required string Name { get; set; }
    public required string Email { get; set; }
    public int Age { get; set; }
    public DateTime EnrollmentDate { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public ObjectId DepartmentId { get; set; }

    // Navigation properties for serialization
    [BsonIgnore]
    public string StringId => Id.ToString();
    
    [BsonIgnore]
    public string StringDepartmentId => DepartmentId.ToString();
}
