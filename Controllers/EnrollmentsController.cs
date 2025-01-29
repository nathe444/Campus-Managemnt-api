using Microsoft.AspNetCore.Mvc;
using CampusManagementSystem.Models;
using CampusManagementSystem.Services;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;  // Add this line
using System.Data;  // Add this line for specific role-based authorization

namespace CampusManagementSystem.Controllers
{
    [Authorize]  // Require authentication for all endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

       public class EnrollmentDto
{
    public string Id { get; set; }
    public string StudentId { get; set; }
    public string CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus Status { get; set; }
    public GradeType? Grade { get; set; }
}

[HttpGet]
[Authorize(Roles = "Admin,Instructor")]
public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetAll()
{
    var enrollments = await _enrollmentService.GetAllAsync();
    var enrollmentDtos = enrollments.Select(e => new EnrollmentDto
    {
        Id = e.Id.ToString(),
        StudentId = e.StudentId.ToString(),
        CourseId = e.CourseId.ToString(),
        EnrollmentDate = e.EnrollmentDate,
        Status = e.Status,
        Grade = e.Grade
    });
    return Ok(enrollmentDtos);
}

[HttpGet("{id}")]
[Authorize(Roles = "Admin,Instructor,Student")]
public async Task<ActionResult<EnrollmentDto>> GetById(string id)
{
    var enrollment = await _enrollmentService.GetByIdAsync(id);
    if (enrollment == null)
        return NotFound();

    // Additional check to ensure students can only view their own enrollments
    if (User.IsInRole("Student"))
    {
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != enrollment.StudentId.ToString())
            return Forbid();
    }

    var enrollmentDto = new EnrollmentDto
    {
        Id = enrollment.Id.ToString(),
        StudentId = enrollment.StudentId.ToString(),
        CourseId = enrollment.CourseId.ToString(),
        EnrollmentDate = enrollment.EnrollmentDate,
        Status = enrollment.Status,
        Grade = enrollment.Grade
    };

    return Ok(enrollmentDto);
}

[HttpGet("student/{studentId}")]
[Authorize(Roles = "Admin,Instructor,Student")]
public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetByStudent(string studentId)
{
    // Additional check to ensure students can only view their own enrollments
    if (User.IsInRole("Student"))
    {
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != studentId)
            return Forbid();
    }

    var enrollments = await _enrollmentService.GetByStudentIdAsync(studentId);
    var enrollmentDtos = enrollments.Select(e => new EnrollmentDto
    {
        Id = e.Id.ToString(),
        StudentId = e.StudentId.ToString(),
        CourseId = e.CourseId.ToString(),
        EnrollmentDate = e.EnrollmentDate,
        Status = e.Status,
        Grade = e.Grade
    });
    return Ok(enrollmentDtos);
}

[HttpGet("course/{courseId}")]
[Authorize(Roles = "Admin,Instructor")]
public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetByCourse(string courseId)
{
    var enrollments = await _enrollmentService.GetByCourseIdAsync(courseId);
    var enrollmentDtos = enrollments.Select(e => new EnrollmentDto
    {
        Id = e.Id.ToString(),
        StudentId = e.StudentId.ToString(),
        CourseId = e.CourseId.ToString(),
        EnrollmentDate = e.EnrollmentDate,
        Status = e.Status,
        Grade = e.Grade
    });
    return Ok(enrollmentDtos);
}

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]  // Only Admins and Instructors can create enrollments
        public async Task<ActionResult<Enrollment>> Create([FromBody] CreateEnrollmentDto enrollmentDto)
        {
            var enrollment = new Enrollment
            {
                StudentId = ObjectId.Parse(enrollmentDto.StudentId),
                CourseId = ObjectId.Parse(enrollmentDto.CourseId),
                EnrollmentDate = enrollmentDto.EnrollmentDate ?? DateTime.UtcNow
            };

            var created = await _enrollmentService.CreateAsync(enrollment);
            return CreatedAtAction(nameof(GetById), new { id = created.Id.ToString() }, created);
        }


        [HttpPut("{id}/grade")]
        [Authorize(Roles = "Admin,Instructor")]  // Only Admins and Instructors can update grades
        public async Task<ActionResult<Enrollment>> UpdateGrade(string id, GradeType grade)
        {
            try
            {
                var updated = await _enrollmentService.UpdateGradeAsync(id, grade);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Only Admins can delete enrollments
        public async Task<IActionResult> Delete(string id)
        {
            await _enrollmentService.DeleteAsync(id);
            return NoContent();
        }

        // Data Transfer Objects remain the same
        public class CreateEnrollmentDto
        {
            public string StudentId { get; set; }
            public string CourseId { get; set; }
            public DateTime? EnrollmentDate { get; set; }
        }

        public class UpdateEnrollmentDto
        {
            public EnrollmentStatus? Status { get; set; }
        }
    }
}