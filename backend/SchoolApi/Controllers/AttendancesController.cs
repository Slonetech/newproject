using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Services;
using System;
using System.Threading.Tasks;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendancesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AttendancesController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendances()
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<Attendance>> GetAttendance(Guid id)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            return attendance;
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetStudentAttendances(Guid studentId)
        {
            return await _context.Attendances
                .Include(a => a.Course)
                .Where(a => a.StudentId == studentId)
                .ToListAsync();
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetCourseAttendances(Guid courseId)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.CourseId == courseId)
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<Attendance>> CreateAttendance(Attendance attendance)
        {
            var student = await _context.Students
                .Include(s => s.Parents)
                .FirstOrDefaultAsync(s => s.Id == attendance.StudentId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            var course = await _context.Courses.FindAsync(attendance.CourseId);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            // Send email notification to parents
            foreach (var parent in student.Parents)
            {
                try
                {
                    await _emailService.SendAttendanceNotificationAsync(
                        parent.Email,
                        student.FirstName,
                        student.LastName,
                        course.Name,
                        attendance.IsPresent,
                        attendance.Date
                    );
                }
                catch (Exception)
                {
                    // Log the error but don't fail the request
                }
            }

            return CreatedAtAction(nameof(GetAttendance), new { id = attendance.Id }, attendance);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> UpdateAttendance(Guid id, Attendance attendance)
        {
            if (id != attendance.Id)
            {
                return BadRequest();
            }

            var existingAttendance = await _context.Attendances
                .Include(a => a.Student)
                .ThenInclude(s => s.Parents)
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingAttendance == null)
            {
                return NotFound();
            }

            _context.Entry(existingAttendance).CurrentValues.SetValues(attendance);

            try
            {
                await _context.SaveChangesAsync();

                // Send email notification to parents
                foreach (var parent in existingAttendance.Student.Parents)
                {
                    try
                    {
                        await _emailService.SendAttendanceNotificationAsync(
                            parent.Email,
                            existingAttendance.Student.FirstName,
                            existingAttendance.Student.LastName,
                            existingAttendance.Course.Name,
                            attendance.IsPresent,
                            attendance.Date
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
                if (!AttendanceExists(id))
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
        public async Task<IActionResult> DeleteAttendance(Guid id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttendanceExists(Guid id)
        {
            return _context.Attendances.Any(e => e.Id == id);
        }
    }
}
