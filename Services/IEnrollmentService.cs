using MongoDB.Bson;
using CampusManagementSystem.Models;

namespace CampusManagementSystem.Services
{
    public interface IEnrollmentService
    {
        Task<Enrollment> CreateAsync(Enrollment enrollment);
        Task<Enrollment?> GetByIdAsync(string id);
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<IEnumerable<Enrollment>> GetByStudentIdAsync(string studentId);
        Task<IEnumerable<Enrollment>> GetByCourseIdAsync(string courseId);
        Task<Enrollment> UpdateGradeAsync(string id, GradeType grade);
        Task DeleteAsync(string id);
    }
}