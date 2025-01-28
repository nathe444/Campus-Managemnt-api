using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Instructor
{
    [BsonId]
    public ObjectId Id { get; set; } 
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTime HireDate { get; set; }
    public ObjectId DepartmentId { get; set; } 
}
