using CampusManagementSystem.Models;

namespace CampusManagementSystem.Services;

public interface IStudentService : IBaseService<Student>
{
    Task<IEnumerable<Student>> GetByDepartmentAsync(string departmentId);
    Task<IEnumerable<Student>> SearchAsync(string searchTerm);
    Task<IEnumerable<Course>> GetEnrolledCoursesAsync(string studentId);
}