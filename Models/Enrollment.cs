using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Enrollment
{
    [BsonId]
    public ObjectId Id { get; set; } 
    public ObjectId StudentId { get; set; } 
    public ObjectId CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public required string Status { get; set; } 
    public required string Grade { get; set; }
}
