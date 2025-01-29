using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CampusManagementSystem.Models
{
    public class Enrollment
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; } 
        
        public ObjectId StudentId { get; set; } 
        public ObjectId CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public EnrollmentStatus Status { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public GradeType? Grade { get; set; }  // Nullable and not required
    }
}