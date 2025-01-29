using CampusManagementSystem.Models;
using CampusManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;


namespace CampusManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<Course>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var courses = await _courseService.GetAllAsync(page, pageSize);
        var total = await _courseService.CountAsync();
        
        return Ok(new {
            data = courses.Select(c => new
            {
                id = c.Id.ToString(),
                c.Title,
                c.Description,
                instructorId = c.InstructorId.ToString(),
                departmentIds = c.DepartmentIds.Select(d => d.ToString()),
                c.StartDate,
                c.EndDate,
                c.Credits
            }),
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Instructor,Student")]
    public async Task<ActionResult<Course>> GetById(string id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = course.Id.ToString(),
            course.Title,
            course.Description,
            instructorId = course.InstructorId.ToString(),
            departmentIds = course.DepartmentIds.Select(d => d.ToString()),
            course.StartDate,
            course.EndDate,
            course.Credits
        });
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<Course>>> Search([FromQuery] string term)
    {
        var courses = await _courseService.SearchAsync(term);
        return Ok(courses.Select(c => new
        {
            id = c.Id.ToString(),
            c.Title,
            c.Description,
            instructorId = c.InstructorId.ToString(),
            departmentIds = c.DepartmentIds.Select(d => d.ToString()),
            c.StartDate,
            c.EndDate,
            c.Credits
        }));
    }

    [HttpGet("instructor/{instructorId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<Course>>> GetByInstructor(string instructorId)
    {
        var courses = await _courseService.GetByInstructorAsync(instructorId);
        return Ok(courses.Select(c => new
        {
            id = c.Id.ToString(),
            c.Title,
            c.Description,
            instructorId = c.InstructorId.ToString(),
            departmentIds = c.DepartmentIds.Select(d => d.ToString()),
            c.StartDate,
            c.EndDate,
            c.Credits
        }));
    }

    [HttpGet("department/{departmentId}")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<ActionResult<IEnumerable<Course>>> GetByDepartment(string departmentId)
    {
        var courses = await _courseService.GetByDepartmentAsync(departmentId);
        return Ok(courses.Select(c => new
        {
            id = c.Id.ToString(),
            c.Title,
            c.Description,
            instructorId = c.InstructorId.ToString(),
            departmentIds = c.DepartmentIds.Select(d => d.ToString()),
            c.StartDate,
            c.EndDate,
            c.Credits
        }));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Course>> Create(CreateCourseDto courseDto)
    {
        var course = new Course
        {
            Title = courseDto.Title,
            Description = courseDto.Description,
            InstructorId = ObjectId.Parse(courseDto.InstructorId),
            DepartmentIds = courseDto.DepartmentIds.Select(id => ObjectId.Parse(id)).ToList(),
            StartDate = courseDto.StartDate,
            EndDate = courseDto.EndDate,
            Credits = courseDto.Credits
        };

        var createdCourse = await _courseService.CreateAsync(course);
        return CreatedAtAction(nameof(GetById), new { id = createdCourse.Id.ToString() }, new
        {
            id = createdCourse.Id.ToString(),
            createdCourse.Title,
            createdCourse.Description,
            instructorId = createdCourse.InstructorId.ToString(),
            departmentIds = createdCourse.DepartmentIds.Select(d => d.ToString()),
            createdCourse.StartDate,
            createdCourse.EndDate,
            createdCourse.Credits
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Course>> Update(string id, UpdateCourseDto courseDto)
    {
        var existingCourse = await _courseService.GetByIdAsync(id);
        if (existingCourse == null)
        {
            return NotFound();
        }

        var course = new Course
        {
            Id = existingCourse.Id,
            Title = courseDto.Title,
            Description = courseDto.Description,
            InstructorId = ObjectId.Parse(courseDto.InstructorId),
            DepartmentIds = courseDto.DepartmentIds.Select(id => ObjectId.Parse(id)).ToList(),
            StartDate = courseDto.StartDate,
            EndDate = courseDto.EndDate,
            Credits = courseDto.Credits
        };

        var updatedCourse = await _courseService.UpdateAsync(id, course);
        return Ok(new
        {
            id = updatedCourse.Id.ToString(),
            updatedCourse.Title,
            updatedCourse.Description,
            instructorId = updatedCourse.InstructorId.ToString(),
            departmentIds = updatedCourse.DepartmentIds.Select(d => d.ToString()),
            updatedCourse.StartDate,
            updatedCourse.EndDate,
            updatedCourse.Credits
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _courseService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}