using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Course
{
    [BsonId]
    public ObjectId Id { get; set; } 
    public string Title { get; set; }
    public string Description { get; set; }
    public ObjectId InstructorId { get; set; } 
    public List<ObjectId> DepartmentIds { get; set; } 
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Credits { get; set; }
}
