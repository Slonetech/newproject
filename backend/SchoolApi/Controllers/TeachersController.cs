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
    [Authorize(Roles = "Admin")]
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
                .Include(t => t.Courses)
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        // GET: api/Teachers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetTeacher(Guid id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .Include(t => t.Courses)
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

            var existingTeacher = await _context.Teachers.FindAsync(id);
            if (existingTeacher == null)
            {
                return NotFound();
            }

            _context.Entry(existingTeacher).CurrentValues.SetValues(teacher);
            existingTeacher.UpdatedAt = DateTime.UtcNow;

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

        // DELETE: api/Teachers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(Guid id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            // Soft delete - mark as inactive
            teacher.IsActive = false;
            teacher.UpdatedAt = DateTime.UtcNow;

            // Remove teacher from courses
            foreach (var course in teacher.Courses)
            {
                course.TeacherId = Guid.Empty;
                course.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Teachers/5/Courses
        [HttpGet("{id}/courses")]
        public async Task<ActionResult<IEnumerable<Course>>> GetTeacherCourses(Guid id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

            if (teacher == null)
            {
                return NotFound();
            }

            return teacher.Courses.ToList();
        }

        // POST: api/Teachers/5/Courses
        [HttpPost("{id}/courses")]
        public async Task<ActionResult<Course>> AssignCourse(Guid id, [FromBody] Course course)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

            if (teacher == null)
            {
                return NotFound("Teacher not found");
            }

            var existingCourse = await _context.Courses.FindAsync(course.Id);
            if (existingCourse == null)
            {
                return NotFound("Course not found");
            }

            existingCourse.TeacherId = id;
            existingCourse.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(existingCourse);
        }

        // DELETE: api/Teachers/5/Courses/3
        [HttpDelete("{teacherId}/courses/{courseId}")]
        public async Task<IActionResult> RemoveCourse(Guid teacherId, Guid courseId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.Id == teacherId && t.IsActive);

            if (teacher == null)
            {
                return NotFound("Teacher not found");
            }

            var course = teacher.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
            {
                return NotFound("Course not found for this teacher");
            }

            course.TeacherId = Guid.Empty;
            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeacherExists(Guid id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}