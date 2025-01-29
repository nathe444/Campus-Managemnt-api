using CampusManagementSystem.Models;

namespace CampusManagementSystem.Services;

public interface IDepartmentService : IBaseService<Department>
{
    Task<IEnumerable<Department>> SearchAsync(string searchTerm);
    Task<bool> ExistsAsync(string id);
    Task<IEnumerable<Student>> GetStudentsAsync(string departmentId);
    Task<IEnumerable<Instructor>> GetInstructorsAsync(string departmentId);
}