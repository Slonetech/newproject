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
    [Authorize] // Adjust authorization as needed for courses
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            // Now including TeacherCourses and then the Teacher from that join table
            return await _context.Courses
                .Include(c => c.StudentCourses)
                    .ThenInclude(sc => sc.Student)
                .Include(c => c.Grades) // Include Grades if needed for reporting or display
                .Include(c => c.Attendances) // Include Attendances if needed
                .Include(c => c.TeacherCourses) // Include the new join table
                    .ThenInclude(tc => tc.Teacher) // Then include the Teacher from the join table
                .Where(c => c.IsActive) // Only return active courses
                .ToListAsync();
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(Guid id)
        {
            var course = await _context.Courses
                .Include(c => c.StudentCourses)
                    .ThenInclude(sc => sc.Student)
                .Include(c => c.Grades) // Include Grades if needed
                .Include(c => c.Attendances) // Include Attendances if needed
                .Include(c => c.TeacherCourses) // Include the new join table
                    .ThenInclude(tc => tc.Teacher) // Then include the Teacher from the join table
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive); // Only retrieve active course

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        // POST: api/Courses
        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admin can create courses
        public async Task<ActionResult<Course>> CreateCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Initialize new properties
            course.Id = Guid.NewGuid();
            course.IsActive = true;
            course.CreatedAt = DateTime.UtcNow;
            // UpdatedAt will be set on subsequent updates

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Return the created course with basic info, or re-fetch to include relations if necessary
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can update courses
        public async Task<IActionResult> UpdateCourse(Guid id, Course course)
        {
            if (id != course.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCourse = await _context.Courses
                                            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive); // Only update active courses

            if (existingCourse == null)
            {
                return NotFound();
            }

            // Update scalar properties
            existingCourse.Name = course.Name;
            existingCourse.Code = course.Code;
            existingCourse.Description = course.Description;
            existingCourse.Credits = course.Credits;
            // existingCourse.TeacherId = course.TeacherId; // This property no longer exists directly on Course
            existingCourse.UpdatedAt = DateTime.UtcNow; // Set update time

            // If you need to update teacher assignments for a course, you'll need separate endpoints
            // like AssignTeacherToCourse, RemoveTeacherFromCourse, or a DTO for course update
            // that includes a list of teacher IDs. For now, this just updates scalar properties.

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Re-throw if it's a genuine concurrency issue
                }
            }

            return NoContent();
        }

        // DELETE: api/Courses/5 (Soft Delete)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete courses
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _context.Courses
                .Include(c => c.TeacherCourses) // Include to remove associations
                .Include(c => c.StudentCourses) // Include to remove associations
                .Include(c => c.Grades) // Include to remove associations
                .Include(c => c.Attendances) // Include to remove associations
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive); // Ensure it's active before soft-deleting

            if (course == null)
            {
                return NotFound();
            }

            // Soft delete - mark as inactive
            course.IsActive = false;
            course.UpdatedAt = DateTime.UtcNow;

            // Remove all TeacherCourse associations for this course
            _context.TeacherCourses.RemoveRange(course.TeacherCourses);

            // Consider cascade deletes for StudentCourses, Grades, Attendances
            // If cascade delete is not configured, you might need to remove them explicitly here:
            _context.StudentCourses.RemoveRange(course.StudentCourses);
            _context.Grades.RemoveRange(course.Grades);
            _context.Attendances.RemoveRange(course.Attendances);


            await _context.SaveChangesAsync();

            return NoContent();
        }

        // New: Assign Teacher(s) to a Course
        [HttpPost("{courseId}/teachers/{teacherId}")]
        [Authorize(Roles = "Admin,Teacher")] // Admin can assign, maybe a lead teacher too
        public async Task<IActionResult> AssignTeacherToCourse(Guid courseId, Guid teacherId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            var teacher = await _context.Teachers.FindAsync(teacherId);

            if (course == null || teacher == null)
            {
                return NotFound("Course or Teacher not found.");
            }

            // Check if association already exists
            if (await _context.TeacherCourses.AnyAsync(tc => tc.CourseId == courseId && tc.TeacherId == teacherId))
            {
                return Conflict("Teacher is already assigned to this course.");
            }

            var teacherCourse = new TeacherCourse
            {
                CourseId = courseId,
                TeacherId = teacherId,
                AssignmentDate = DateTime.UtcNow
            };

            _context.TeacherCourses.Add(teacherCourse);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Teacher assigned to course successfully." });
        }

        // New: Remove Teacher from a Course
        [HttpDelete("{courseId}/teachers/{teacherId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> RemoveTeacherFromCourse(Guid courseId, Guid teacherId)
        {
            var teacherCourse = await _context.TeacherCourses
                .FirstOrDefaultAsync(tc => tc.CourseId == courseId && tc.TeacherId == teacherId);

            if (teacherCourse == null)
            {
                return NotFound("Teacher-Course association not found.");
            }

            _context.TeacherCourses.Remove(teacherCourse);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Teacher removed from course successfully." });
        }


        // GET: api/Courses/5/Teachers (New endpoint to get teachers for a course)
        [HttpGet("{courseId}/teachers")]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetCourseTeachers(Guid courseId)
        {
            var course = await _context.Courses
                .Include(c => c.TeacherCourses)
                    .ThenInclude(tc => tc.Teacher)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.IsActive);

            if (course == null)
            {
                return NotFound("Course not found or is inactive.");
            }

            // Select only the Teacher objects from the join table
            return course.TeacherCourses.Select(tc => tc.Teacher).ToList();
        }


        private bool CourseExists(Guid id)
        {
            return _context.Courses.Any(e => e.Id == id && e.IsActive);
        }
    }
}