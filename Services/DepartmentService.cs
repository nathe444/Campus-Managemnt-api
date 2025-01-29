using CampusManagementSystem.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace CampusManagementSystem.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IMongoCollection<Department> _departments;
    private readonly IMongoCollection<Student> _students;
    private readonly IMongoCollection<Instructor> _instructors;

    public DepartmentService(MongoDBContext dbContext)
    {
        _departments = dbContext.Departments;
        _students = dbContext.Students;
        _instructors = dbContext.Instructors;
    }

    public async Task<Department> CreateAsync(Department department)
    {
        await _departments.InsertOneAsync(department);
        return department;
    }

    public async Task<Department?> GetByIdAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        return await _departments.Find(d => d.Id == objectId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Department>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        return await _departments.Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<Department> UpdateAsync(string id, Department department)
    {
        var objectId = ObjectId.Parse(id);
        department.Id = objectId;
        await _departments.ReplaceOneAsync(d => d.Id == objectId, department);
        return department;
    }

    public async Task DeleteAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        await _departments.DeleteOneAsync(d => d.Id == objectId);
    }

    public async Task<IEnumerable<Department>> FindAsync(Expression<Func<Department, bool>> filter)
    {
        return await _departments.Find(filter).ToListAsync();
    }

    public async Task<long> CountAsync()
    {
        return await _departments.CountDocumentsAsync(_ => true);
    }

    public async Task<IEnumerable<Department>> SearchAsync(string searchTerm)
    {
        var filter = Builders<Department>.Filter.Regex(d => d.Name, new BsonRegularExpression(searchTerm, "i"));
        return await _departments.Find(filter).ToListAsync();
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var objectId = ObjectId.Parse(id);
        return await _departments.Find(d => d.Id == objectId).AnyAsync();
    }

    public async Task<IEnumerable<Student>> GetStudentsAsync(string departmentId)
    {
        var objectId = ObjectId.Parse(departmentId);
        return await _students.Find(s => s.DepartmentId == objectId).ToListAsync();
    }

    public async Task<IEnumerable<Instructor>> GetInstructorsAsync(string departmentId)
    {
        var objectId = ObjectId.Parse(departmentId);
        return await _instructors.Find(i => i.DepartmentId == objectId).ToListAsync();
    }
}