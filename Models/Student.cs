using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Student
{
    [BsonId]
    public ObjectId Id { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public ObjectId DepartmentId { get; set; } 
}
