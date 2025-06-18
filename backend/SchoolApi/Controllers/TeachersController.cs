using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Ensure proper authorization
    public class TeachersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeachersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Teachers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachers()
        {
            return await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.TeacherCourses) // Include the many-to-many join table
                    .ThenInclude(tc => tc.Course) // Then include the Course from the join table
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        // GET: api/Teachers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetTeacher(Guid id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.TeacherCourses) // Include the many-to-many join table
                    .ThenInclude(tc => tc.Course) // Then include the Course from the join table
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

            if (teacher == null)
            {
                return NotFound();
            }

            return teacher;
        }

        // POST: api/Teachers
        [HttpPost]
        public async Task<ActionResult<Teacher>> CreateTeacher(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            teacher.Id = Guid.NewGuid();
            teacher.HireDate = DateTime.UtcNow;
            teacher.IsActive = true;
            teacher.CreatedAt = DateTime.UtcNow; // Ensure CreatedAt is set

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, teacher);
        }

        // PUT: api/Teachers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(Guid id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTeacher = await _context.Teachers
                .Include(t => t.TeacherCourses) // Include for potential updates to assignments
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTeacher == null)
            {
                return NotFound();
            }

            // Update scalar properties
            _context.Entry(existingTeacher).CurrentValues.SetValues(teacher);
            existingTeacher.UpdatedAt = DateTime.UtcNow;

            // Handle TeacherCourse assignments (this is a more complex logic and might require DTOs)
            // For simplicity here, we assume if you're updating a teacher, you might not be directly
            // updating their course assignments through this endpoint. If you are,
            // you'd need to compare existingTeacher.TeacherCourses with teacher.TeacherCourses
            // and add/remove accordingly. For now, we'll assume a separate endpoint for assignments.

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Teachers/5 (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(Guid id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.TeacherCourses) // Include TeacherCourses to handle associated assignments
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            // Soft delete - mark as inactive
            teacher.IsActive = false;
            teacher.UpdatedAt = DateTime.UtcNow;

            // Disassociate from courses by marking TeacherCourses as inactive (if TeacherCourse had IsActive)
            // Or, if a TeacherCourse implies active assignment, you might delete them or handle them differently
            // Here, we'll effectively remove the association by deleting the TeacherCourse entries
            _context.TeacherCourses.RemoveRange(teacher.TeacherCourses);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Teachers/5/Courses
        [HttpGet("{id}/courses")]
        public async Task<ActionResult<IEnumerable<Course>>> GetTeacherAssignedCourses(Guid id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.TeacherCourses)
                    .ThenInclude(tc => tc.Course)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

            if (teacher == null)
            {
                return NotFound("Teacher not found or is inactive.");
            }

            return teacher.TeacherCourses.Select(tc => tc.Course).ToList();
        }

        // POST: api/Teachers/5/Courses/{courseId} - Assign a course to a teacher
        [HttpPost("{teacherId}/courses/{courseId}")]
        public async Task<IActionResult> AssignCourseToTeacher(Guid teacherId, Guid courseId)
        {
            var teacher = await _context.Teachers.FindAsync(teacherId);
            var course = await _context.Courses.FindAsync(courseId);

            if (teacher == null || !teacher.IsActive)
            {
                return NotFound("Teacher not found or is inactive.");
            }

            if (course == null || !course.IsActive)
            {
                return NotFound("Course not found or is inactive.");
            }

            // Check if the assignment already exists
            var existingAssignment = await _context.TeacherCourses
                .AnyAsync(tc => tc.TeacherId == teacherId && tc.CourseId == courseId);

            if (existingAssignment)
            {
                return Conflict("This course is already assigned to this teacher.");
            }

            var teacherCourse = new TeacherCourse
            {
                TeacherId = teacherId,
                CourseId = courseId,
                AssignmentDate = DateTime.UtcNow
            };

            _context.TeacherCourses.Add(teacherCourse);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeacherAssignedCourses), new { id = teacherId }, teacherCourse);
        }

        // DELETE: api/Teachers/5/Courses/{courseId} - Remove a course from a teacher
        [HttpDelete("{teacherId}/courses/{courseId}")]
        public async Task<IActionResult> RemoveCourseFromTeacher(Guid teacherId, Guid courseId)
        {
            var teacherCourse = await _context.TeacherCourses
                .FirstOrDefaultAsync(tc => tc.TeacherId == teacherId && tc.CourseId == courseId);

            if (teacherCourse == null)
            {
                return NotFound("Assignment not found for this teacher and course.");
            }

            _context.TeacherCourses.Remove(teacherCourse);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool TeacherExists(Guid id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}