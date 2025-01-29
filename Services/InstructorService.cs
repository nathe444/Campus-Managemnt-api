using CampusManagementSystem.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace CampusManagementSystem.Services;

public class InstructorService : IInstructorService
{
    private readonly IMongoCollection<Instructor> _instructors;
    private readonly IMongoCollection<Course> _courses;

    public InstructorService(MongoDBContext dbContext)
    {
        _instructors = dbContext.Instructors;
        _courses = dbContext.Courses;
    }

    public async Task<Instructor> CreateAsync(Instructor instructor)
    {
        await _instructors.InsertOneAsync(instructor);
        return instructor;
    }

    public async Task<Instructor?> GetByIdAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        return await _instructors.Find(i => i.Id == objectId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Instructor>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        return await _instructors.Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<Instructor> UpdateAsync(string id, Instructor instructor)
    {
        var objectId = ObjectId.Parse(id);
        instructor.Id = objectId;
        await _instructors.ReplaceOneAsync(i => i.Id == objectId, instructor);
        return instructor;
    }

    public async Task DeleteAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        await _instructors.DeleteOneAsync(i => i.Id == objectId);
    }

    public async Task<IEnumerable<Instructor>> FindAsync(Expression<Func<Instructor, bool>> filter)
    {
        return await _instructors.Find(filter).ToListAsync();
    }

    public async Task<long> CountAsync()
    {
        return await _instructors.CountDocumentsAsync(_ => true);
    }

    public async Task<IEnumerable<Instructor>> GetByDepartmentAsync(string departmentId)
    {
        var objectId = ObjectId.Parse(departmentId);
        return await _instructors.Find(i => i.DepartmentId == objectId).ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetAssignedCoursesAsync(string instructorId)
    {
        var objectId = ObjectId.Parse(instructorId);
        return await _courses.Find(c => c.InstructorId == objectId).ToListAsync();
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        return await _instructors.Find(i => i.Id == objectId).AnyAsync();
    }

    public async Task<IEnumerable<Instructor>> SearchAsync(string searchTerm)
    {
        var filter = Builders<Instructor>.Filter.Or(
            Builders<Instructor>.Filter.Regex(i => i.Name, new BsonRegularExpression(searchTerm, "i")),
            Builders<Instructor>.Filter.Regex(i => i.Email, new BsonRegularExpression(searchTerm, "i"))
        );
        return await _instructors.Find(filter).ToListAsync();
    }
}