using CampusManagementSystem.Models;

namespace CampusManagementSystem.Services;

public interface IInstructorService : IBaseService<Instructor>
{
    Task<IEnumerable<Instructor>> GetByDepartmentAsync(string departmentId);
    Task<IEnumerable<Course>> GetAssignedCoursesAsync(string instructorId);
    Task<bool> ExistsAsync(string id);
    Task<IEnumerable<Instructor>> SearchAsync(string searchTerm);
}