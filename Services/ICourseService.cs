using CampusManagementSystem.Models;
using System.Linq.Expressions;

namespace CampusManagementSystem.Services;

public interface ICourseService
{
    Task<Course> CreateAsync(Course course);
    Task<Course?> GetByIdAsync(string id);
    Task<IEnumerable<Course>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<Course> UpdateAsync(string id, Course course);
    Task DeleteAsync(string id);
    Task<long> CountAsync();
    Task<IEnumerable<Course>> FindAsync(Expression<Func<Course, bool>> filter);
    Task<IEnumerable<Course>> GetByInstructorAsync(string instructorId);
    Task<IEnumerable<Course>> GetByDepartmentAsync(string departmentId);
    Task<IEnumerable<Course>> SearchAsync(string searchTerm);
}