using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Instructor
{
    [BsonId]
    public ObjectId Id { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime HireDate { get; set; }
    public ObjectId DepartmentId { get; set; } 
}
