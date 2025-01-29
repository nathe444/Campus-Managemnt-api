using CampusManagementSystem.Models;
using CampusManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<Student>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var students = await _studentService.GetAllAsync(page, pageSize);
        var total = await _studentService.CountAsync();
        
        return Ok(new {
            data = students.Select(s => new
            {
                id = s.StringId,
                s.Name,
                s.Email,
                s.Age,
                s.EnrollmentDate,
                departmentId = s.StringDepartmentId
            }),
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Student>> GetById(string id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        // Only allow students to view their own profile
        if (User.IsInRole("Student") && User.FindFirst("ProfileId")?.Value != id)
        {
            return Forbid();
        }

        return Ok(new
        {
            id = student.StringId,
            student.Name,
            student.Email,
            student.Age,
            student.EnrollmentDate,
            departmentId = student.StringDepartmentId
        });
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<Student>>> Search([FromQuery] string term)
    {
        var students = await _studentService.SearchAsync(term);
        return Ok(students.Select(s => new
        {
            id = s.StringId,
            s.Name,
            s.Email,
            s.Age,
            s.EnrollmentDate,
            departmentId = s.StringDepartmentId
        }));
    }

    [HttpGet("department/{departmentId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<Student>>> GetByDepartment(string departmentId)
    {
        var students = await _studentService.GetByDepartmentAsync(departmentId);
        return Ok(students.Select(s => new
        {
            id = s.StringId,
            s.Name,
            s.Email,
            s.Age,
            s.EnrollmentDate,
            departmentId = s.StringDepartmentId
        }));
    }

    [HttpGet("{id}/courses")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Course>>> GetEnrolledCourses(string id)
    {
        // Only allow students to view their own courses
        if (User.IsInRole("Student") && User.FindFirst("ProfileId")?.Value != id)
        {
            return Forbid();
        }

        var courses = await _studentService.GetEnrolledCoursesAsync(id);
        return Ok(courses);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Student>> Update(string id, Student student)
    {
        var existingStudent = await _studentService.GetByIdAsync(id);
        if (existingStudent == null)
        {
            return NotFound();
        }

        // Ensure Id and DepartmentId are not changed
        student.Id = existingStudent.Id;
        student.DepartmentId = existingStudent.DepartmentId;

        var updatedStudent = await _studentService.UpdateAsync(id, student);
        return Ok(new
        {
            id = updatedStudent.StringId,
            updatedStudent.Name,
            updatedStudent.Email,
            updatedStudent.Age,
            updatedStudent.EnrollmentDate,
            departmentId = updatedStudent.StringDepartmentId
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _studentService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}