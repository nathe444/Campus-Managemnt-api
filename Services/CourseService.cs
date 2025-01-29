using CampusManagementSystem.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace CampusManagementSystem.Services;

public class CourseService : ICourseService
{
    private readonly IMongoCollection<Course> _courses;

    public CourseService(MongoDBContext dbContext)
    {
        _courses = dbContext.Courses;
    }

    public async Task<Course> CreateAsync(Course course)
    {
        await _courses.InsertOneAsync(course);
        return course;
    }

    public async Task<Course?> GetByIdAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        return await _courses.Find(c => c.Id == objectId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Course>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        return await _courses.Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<Course> UpdateAsync(string id, Course course)
    {
        var objectId = ObjectId.Parse(id);
        var update = Builders<Course>.Update
            .Set(c => c.Title, course.Title)
            .Set(c => c.Description, course.Description)
            .Set(c => c.InstructorId, course.InstructorId)
            .Set(c => c.DepartmentIds, course.DepartmentIds)
            .Set(c => c.StartDate, course.StartDate)
            .Set(c => c.EndDate, course.EndDate)
            .Set(c => c.Credits, course.Credits);

        await _courses.UpdateOneAsync(c => c.Id == objectId, update);
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        await _courses.DeleteOneAsync(c => c.Id == objectId);
    }

    public async Task<long> CountAsync()
    {
        return await _courses.CountDocumentsAsync(_ => true);
    }

    public async Task<IEnumerable<Course>> FindAsync(Expression<Func<Course, bool>> filter)
    {
        return await _courses.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetByInstructorAsync(string instructorId)
    {
        var objectId = ObjectId.Parse(instructorId);
        return await _courses.Find(c => c.InstructorId == objectId).ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetByDepartmentAsync(string departmentId)
    {
        var objectId = ObjectId.Parse(departmentId);
        return await _courses.Find(c => c.DepartmentIds.Contains(objectId)).ToListAsync();
    }

    public async Task<IEnumerable<Course>> SearchAsync(string searchTerm)
    {
        var filter = Builders<Course>.Filter.Or(
            Builders<Course>.Filter.Regex(c => c.Title, new BsonRegularExpression(searchTerm, "i")),
            Builders<Course>.Filter.Regex(c => c.Description, new BsonRegularExpression(searchTerm, "i"))
        );
        return await _courses.Find(filter).ToListAsync();
    }
}