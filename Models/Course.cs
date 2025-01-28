using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Course
{
    [BsonId]
    public ObjectId Id { get; set; } 
    public required string Title { get; set; }
    public required string Description { get; set; }
    public ObjectId InstructorId { get; set; } 
    public required List<ObjectId> DepartmentIds { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required int Credits { get; set; }
}
