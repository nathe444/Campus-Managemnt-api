using CampusManagementSystem.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace CampusManagementSystem.Services;

public class StudentService : IStudentService
{
    private readonly IMongoCollection<Student> _students;
    private readonly IMongoCollection<Enrollment> _enrollments;
    private readonly IMongoCollection<Course> _courses;

    public StudentService(MongoDBContext dbContext)
    {
        _students = dbContext.Students;
        _enrollments = dbContext.Enrollments;
        _courses = dbContext.Courses;
    }

    public async Task<Student> CreateAsync(Student student)
    {
        await _students.InsertOneAsync(student);
        return student;
    }

    public async Task<Student?> GetByIdAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        return await _students.Find(s => s.Id == objectId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Student>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        return await _students.Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<Student> UpdateAsync(string id, Student student)
    {
        var objectId = ObjectId.Parse(id);
        student.Id = objectId;
        await _students.ReplaceOneAsync(s => s.Id == objectId, student);
        return student;
    }

    public async Task DeleteAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        await _students.DeleteOneAsync(s => s.Id == objectId);
    }

    public async Task<IEnumerable<Student>> FindAsync(Expression<Func<Student, bool>> filter)
    {
        return await _students.Find(filter).ToListAsync();
    }

    public async Task<long> CountAsync()
    {
        return await _students.CountDocumentsAsync(_ => true);
    }

    public async Task<IEnumerable<Student>> GetByDepartmentAsync(string departmentId)
    {
        var objectId = ObjectId.Parse(departmentId);
        return await _students.Find(s => s.DepartmentId == objectId).ToListAsync();
    }

    public async Task<IEnumerable<Student>> SearchAsync(string searchTerm)
    {
        var filter = Builders<Student>.Filter.Or(
            Builders<Student>.Filter.Regex(s => s.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Student>.Filter.Regex(s => s.Email, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
        );
        return await _students.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetEnrolledCoursesAsync(string studentId)
    {
        var objectId = ObjectId.Parse(studentId);
        var enrollments = await _enrollments.Find(e => e.StudentId == objectId).ToListAsync();
        var courseIds = enrollments.Select(e => e.CourseId);
        return await _courses.Find(c => courseIds.Contains(c.Id)).ToListAsync();
    }
}