using CampusManagementSystem.Models;
using CampusManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Department>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var departments = await _departmentService.GetAllAsync(page, pageSize);
        var total = await _departmentService.CountAsync();
        
        return Ok(new {
            data = departments.Select(d => new
            {
                id = d.StringId,
                d.Name,
                d.Code,
                d.Description,
                d.CreatedAt,
                d.UpdatedAt,
                d.IsActive,
                d.HeadOfDepartment,
                d.HeadInstructorId,
                d.Email,
                d.Phone,
                d.MaxStudents
            }),
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Department>> GetById(string id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        if (department == null)
        {
            return NotFound();
        }
        return Ok(new
        {
            id = department.StringId,
            department.Name,
            department.Code,
            department.Description,
            department.CreatedAt,
            department.UpdatedAt,
            department.IsActive,
            department.HeadOfDepartment,
            department.HeadInstructorId,
            department.Email,
            department.Phone,
            department.MaxStudents
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Department>> Create(CreateDepartmentRequest request)
    {
        var department = new Department
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            HeadOfDepartment = request.HeadOfDepartment,
            Email = request.Email,
            Phone = request.Phone,
            MaxStudents = request.MaxStudents ?? 100
        };

        await _departmentService.CreateAsync(department);
        
        return CreatedAtAction(nameof(GetById), 
            new { id = department.StringId }, 
            new
            {
                id = department.StringId,
                department.Name,
                department.Code,
                department.Description,
                department.CreatedAt,
                department.UpdatedAt,
                department.IsActive,
                department.HeadOfDepartment,
                department.HeadInstructorId,
                department.Email,
                department.Phone,
                department.MaxStudents
            });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Department>> Update(string id, UpdateDepartmentRequest request)
    {
        var department = await _departmentService.GetByIdAsync(id);
        if (department == null)
        {
            return NotFound();
        }

        // Update only the provided fields
        if (request.Name != null) department.Name = request.Name;
        if (request.Code != null) department.Code = request.Code;
        if (request.Description != null) department.Description = request.Description;
        if (request.HeadOfDepartment != null) department.HeadOfDepartment = request.HeadOfDepartment;
        if (request.Email != null) department.Email = request.Email;
        if (request.Phone != null) department.Phone = request.Phone;
        if (request.MaxStudents.HasValue) department.MaxStudents = request.MaxStudents.Value;
        if (request.IsActive.HasValue) department.IsActive = request.IsActive.Value;
        
        department.UpdatedAt = DateTime.UtcNow;

        var updatedDepartment = await _departmentService.UpdateAsync(id, department);
        return Ok(new
        {
            id = updatedDepartment.StringId,
            updatedDepartment.Name,
            updatedDepartment.Code,
            updatedDepartment.Description,
            updatedDepartment.CreatedAt,
            updatedDepartment.UpdatedAt,
            updatedDepartment.IsActive,
            updatedDepartment.HeadOfDepartment,
            updatedDepartment.HeadInstructorId,
            updatedDepartment.Email,
            updatedDepartment.Phone,
            updatedDepartment.MaxStudents
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _departmentService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Department>>> Search([FromQuery] string term)
    {
        var departments = await _departmentService.SearchAsync(term);
        return Ok(departments.Select(d => new
        {
            id = d.StringId,
            d.Name,
            d.Code,
            d.Description,
            d.CreatedAt,
            d.UpdatedAt,
            d.IsActive,
            d.HeadOfDepartment,
            d.HeadInstructorId,
            d.Email,
            d.Phone,
            d.MaxStudents
        }));
    }

    [HttpGet("{id}/students")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudents(string id)
    {
        var students = await _departmentService.GetStudentsAsync(id);
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

    [HttpGet("{id}/instructors")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Instructor>>> GetInstructors(string id)
    {
        var instructors = await _departmentService.GetInstructorsAsync(id);
        return Ok(instructors);
    }
}