using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using CampusManagementSystem.Models;

namespace CampusManagementSystem;

public class MongoDBContext
{
    private readonly IMongoDatabase _database;
    private readonly IConfiguration _configuration;

    public MongoDBContext(IConfiguration configuration)
    {
        _configuration = configuration;

        var connectionString = _configuration.GetSection("MongoDB:ConnectionString").Value;
        var databaseName = _configuration.GetSection("MongoDB:DatabaseName").Value;

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName); 
    }

    public IMongoDatabase Database => _database;
    public IMongoCollection<Student> Students => _database.GetCollection<Student>("Students");
    public IMongoCollection<Course> Courses => _database.GetCollection<Course>("Courses");
    public IMongoCollection<Instructor> Instructors => _database.GetCollection<Instructor>("Instructors");
    public IMongoCollection<Department> Departments => _database.GetCollection<Department>("Departments");
    public IMongoCollection<Enrollment> Enrollments => _database.GetCollection<Enrollment>("Enrollments");
    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");

    public async Task<bool> TestConnection()
    {
        try
        {
            await _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
