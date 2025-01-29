using MongoDB.Driver;
using MongoDB.Bson;
using CampusManagementSystem.Models;

namespace CampusManagementSystem.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IMongoCollection<Enrollment> _enrollments;

        public EnrollmentService(MongoDBContext dbContext)
        {
            _enrollments = dbContext.Enrollments;
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            enrollment.Status = EnrollmentStatus.Active;
            enrollment.Grade = GradeType.IP;  // Set initial grade as In Progress
            await _enrollments.InsertOneAsync(enrollment);
            return enrollment;
        }

        public async Task<Enrollment?> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _enrollments.Find(e => e.Id == objectId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _enrollments.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(string studentId)
        {
            var objectId = ObjectId.Parse(studentId);
            return await _enrollments.Find(e => e.StudentId == objectId).ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(string courseId)
        {
            var objectId = ObjectId.Parse(courseId);
            return await _enrollments.Find(e => e.CourseId == objectId).ToListAsync();
        }


        public async Task<Enrollment> UpdateGradeAsync(string id, GradeType grade)
        {
            var objectId = ObjectId.Parse(id);
            var enrollment = await GetByIdAsync(id);
            
            if (enrollment == null)
                throw new KeyNotFoundException("Enrollment not found");
                
            if (enrollment.Status != EnrollmentStatus.Active)
                throw new InvalidOperationException("Cannot update grade for non-active or non-completed enrollment");

            var update = Builders<Enrollment>.Update
                .Set(e => e.Grade, grade)
                .Set(e => e.Status, EnrollmentStatus.Completed);

            await _enrollments.UpdateOneAsync(e => e.Id == objectId, update);
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            await _enrollments.DeleteOneAsync(e => e.Id == objectId);
        }
    }
}