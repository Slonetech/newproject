using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.DTOs;
using SchoolApi.Models;

namespace SchoolApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GradesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Grades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GradeDto>>> GetGrades()
        {
            var grades = await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    Score = g.Score,
                    DateAwarded = g.DateAwarded,
                    StudentId = g.StudentId,
                    TeacherId = g.TeacherId,
                    CourseId = g.CourseId,
                    StudentName = g.Student.User.FirstName + " " + g.Student.User.LastName,
                    CourseName = g.Course.Name
                })
                .ToListAsync();

            return Ok(grades);
        }

        // GET: api/Grades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GradeDto>> GetGrade(int id)
        {
            var grade = await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    Score = g.Score,
                    DateAwarded = g.DateAwarded,
                    StudentId = g.StudentId,
                    TeacherId = g.TeacherId,
                    CourseId = g.CourseId,
                    StudentName = g.Student.User.FirstName + " " + g.Student.User.LastName,
                    CourseName = g.Course.Name
                })
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade == null)
                return NotFound();

            return Ok(grade);
        }

        // POST: api/Grades
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<GradeDto>> CreateGrade(GradeCreateDto dto)
        {
            var grade = new Grade
            {
                Score = dto.Score,
                DateAwarded = dto.DateAwarded,
                StudentId = dto.StudentId,
                TeacherId = dto.TeacherId,
                CourseId = dto.CourseId
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            var result = new GradeDto
            {
                Id = grade.Id,
                Score = grade.Score,
                DateAwarded = grade.DateAwarded,
                StudentId = grade.StudentId,
                TeacherId = grade.TeacherId,
                CourseId = grade.CourseId
            };

            return CreatedAtAction(nameof(GetGrade), new { id = grade.Id }, result);
        }

        // PUT: api/Grades/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> UpdateGrade(int id, GradeCreateDto dto)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
                return NotFound();

            grade.Score = dto.Score;
            grade.DateAwarded = dto.DateAwarded;
            grade.StudentId = dto.StudentId;
            grade.TeacherId = dto.TeacherId;
            grade.CourseId = dto.CourseId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Grades/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
                return NotFound();

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
