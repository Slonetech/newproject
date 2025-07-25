using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Services;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GradesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public GradesController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grade>>> GetGrades()
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Grade>> GetGrade(Guid id)
        {
            var grade = await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade == null)
            {
                return NotFound();
            }

            return grade;
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<Grade>>> GetStudentGrades(Guid studentId)
        {
            return await _context.Grades
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId)
                .ToListAsync();
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<Grade>>> GetCourseGrades(Guid courseId)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Where(g => g.CourseId == courseId)
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<Grade>> CreateGrade(Grade grade)
        {
            var student = await _context.Students
                .Include(s => s.ParentLinks)
                .FirstOrDefaultAsync(s => s.Id == grade.StudentId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var course = await _context.Courses.FindAsync(grade.CourseId);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            // Send email notification to parents
            var parentLinks = await _context.ParentChildren.Where(pc => pc.StudentId == student.Id).ToListAsync();
            foreach (var link in parentLinks)
            {
                var parent = await _context.Parents.FindAsync(link.ParentId);
                if (parent != null)
                {
                    try
                    {
                        await _emailService.SendGradeNotificationAsync(
                            parent.Email,
                            student.FirstName,
                            student.LastName,
                            course.Name,
                            grade.Value,
                            grade.Date
                        );
                    }
                    catch (Exception)
                    {
                        // Log the error but don't fail the request
                    }
                }
            }

            return CreatedAtAction(nameof(GetGrade), new { id = grade.Id }, grade);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> UpdateGrade(Guid id, Grade grade)
        {
            if (id != grade.Id)
            {
                return BadRequest();
            }

            var existingGrade = await _context.Grades
                .Include(g => g.Student)
                    .ThenInclude(s => s.ParentLinks)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (existingGrade == null)
            {
                return NotFound();
            }

            _context.Entry(existingGrade).CurrentValues.SetValues(grade);

            try
            {
                await _context.SaveChangesAsync();

                // Send email notification to parents
                foreach (var parent in existingGrade.Student.Parents)
                {
                    try
                    {
                        await _emailService.SendGradeNotificationAsync(
                            parent.Email,
                            existingGrade.Student.FirstName,
                            existingGrade.Student.LastName,
                            existingGrade.Course.Name,
                            grade.Value,
                            grade.Date
                        );
                    }
                    catch (Exception)
                    {
                        // Log the error but don't fail the request
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GradeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGrade(Guid id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GradeExists(Guid id)
        {
            return _context.Grades.Any(e => e.Id == id);
        }
    }
}
