using CampusManagementSystem.Models;
using CampusManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace CampusManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Instructor>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var instructors = await _instructorService.GetAllAsync(page, pageSize);
        var total = await _instructorService.CountAsync();
        
        return Ok(new {
            data = instructors.Select(i => new
            {
                id = i.StringId,
                i.Name,
                i.Email,
                departmentId = i.StringDepartmentId,
                i.HireDate,

                i.IsActive,
                i.CreatedAt,
                i.UpdatedAt
            }),
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Instructor>> GetById(string id)
    {
        var instructor = await _instructorService.GetByIdAsync(id);
        if (instructor == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = instructor.StringId,
            instructor.Name,
            instructor.Email,
            departmentId = instructor.StringDepartmentId,
            instructor.HireDate,
            instructor.IsActive,
            instructor.CreatedAt,
            instructor.UpdatedAt
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Instructor>> Update(string id, UpdateInstructorRequest request)
    {
        var instructor = await _instructorService.GetByIdAsync(id);
        if (instructor == null)
        {
            return NotFound();
        }

        if (request.Name != null) instructor.Name = request.Name;
        if (request.Email != null) instructor.Email = request.Email;
        if (request.DepartmentId != null) instructor.DepartmentId = ObjectId.Parse(request.DepartmentId);
        if (request.HireDate.HasValue) instructor.HireDate = request.HireDate.Value;
        if (request.IsActive.HasValue) instructor.IsActive = request.IsActive.Value;
        
        instructor.UpdatedAt = DateTime.UtcNow;

        var updatedInstructor = await _instructorService.UpdateAsync(id, instructor);
        return Ok(new
        {
            id = updatedInstructor.StringId,
            updatedInstructor.Name,
            updatedInstructor.Email,
            departmentId = updatedInstructor.StringDepartmentId,
            updatedInstructor.HireDate,
            updatedInstructor.IsActive,
            updatedInstructor.CreatedAt,
            updatedInstructor.UpdatedAt
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _instructorService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Instructor>>> Search([FromQuery] string term)
    {
        var instructors = await _instructorService.SearchAsync(term);
        return Ok(instructors.Select(i => new
        {
            id = i.StringId,
            i.Name,
            i.Email,
            departmentId = i.StringDepartmentId,
            i.HireDate,
            i.IsActive,
            i.CreatedAt,
            i.UpdatedAt
        }));
    }

    [HttpGet("{id}/courses")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Course>>> GetAssignedCourses(string id)
    {
        var courses = await _instructorService.GetAssignedCoursesAsync(id);
        return Ok(courses);
    }

    [HttpGet("department/{departmentId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Instructor>>> GetByDepartment(string departmentId)
    {
        var instructors = await _instructorService.GetByDepartmentAsync(departmentId);
        return Ok(instructors.Select(i => new
        {
            id = i.StringId,
            i.Name,
            i.Email,
            departmentId = i.StringDepartmentId,
            i.HireDate,
            i.IsActive,
            i.CreatedAt,
            i.UpdatedAt
        }));
    }
}