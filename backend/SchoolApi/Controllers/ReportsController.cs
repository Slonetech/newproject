using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs.Reports;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("student/{id}")]
        public async Task<ActionResult<StudentProgressReport>> GetStudentProgress(Guid id)
        {
            var student = await _context.Students
                .Include(s => s.Grades)
                .ThenInclude(g => g.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            var report = new StudentProgressReport
            {
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                Courses = student.Grades
                    .GroupBy(g => g.Course)
                    .Select(g => new CourseProgress
                    {
                        CourseId = g.Key.Id,
                        CourseName = g.Key.Name,
                        AverageGrade = g.Average(grade => grade.Value),
                        LastGrade = g.OrderByDescending(grade => grade.Date).First().Value,
                        LastGradeDate = g.OrderByDescending(grade => grade.Date).First().Date
                    })
                    .ToList()
            };

            return report;
        }

        [HttpGet("parent/{id}")]
        public async Task<ActionResult<IEnumerable<StudentProgressReport>>> GetParentReport(Guid id)
        {
            var parent = await _context.Parents
                .Include(p => p.Students)
                .ThenInclude(s => s.Grades)
                .ThenInclude(g => g.Course)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parent == null)
            {
                return NotFound();
            }

            var reports = parent.Students.Select(student => new StudentProgressReport
            {
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                Courses = student.Grades
                    .GroupBy(g => g.Course)
                    .Select(g => new CourseProgress
                    {
                        CourseId = g.Key.Id,
                        CourseName = g.Key.Name,
                        AverageGrade = g.Average(grade => grade.Value),
                        LastGrade = g.OrderByDescending(grade => grade.Date).First().Value,
                        LastGradeDate = g.OrderByDescending(grade => grade.Date).First().Date
                    })
                    .ToList()
            });

            return Ok(reports);
        }

        [HttpGet("course/{id}")]
        public async Task<ActionResult<CourseSummaryReport>> GetCourseSummary(Guid id)
        {
            var course = await _context.Courses
                .Include(c => c.Grades)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var grades = course.Grades.Select(g => g.Value).ToList();
            var report = new CourseSummaryReport
            {
                CourseId = course.Id,
                CourseName = course.Name,
                TotalStudents = grades.Count,
                AverageGrade = grades.Any() ? grades.Average() : 0,
                HighestGrade = grades.Any() ? grades.Max() : 0,
                LowestGrade = grades.Any() ? grades.Min() : 0,
                GradeDistribution = new List<GradeDistribution>
                {
                    new() { Range = "90-100", Count = grades.Count(g => g >= 90) },
                    new() { Range = "80-89", Count = grades.Count(g => g >= 80 && g < 90) },
                    new() { Range = "70-79", Count = grades.Count(g => g >= 70 && g < 80) },
                    new() { Range = "60-69", Count = grades.Count(g => g >= 60 && g < 70) },
                    new() { Range = "0-59", Count = grades.Count(g => g < 60) }
                }
            };

            return report;
        }

        [HttpGet("attendance")]
        public async Task<ActionResult<AttendanceSummaryReport>> GetAttendanceSummary(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var attendances = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .ToListAsync();

            var report = new AttendanceSummaryReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalDays = (endDate - startDate).Days + 1,
                CourseBreakdown = attendances
                    .GroupBy(a => a.Course)
                    .Select(g => new CourseAttendance
                    {
                        CourseId = g.Key.Id,
                        CourseName = g.Key.Name,
                        TotalStudents = g.Select(a => a.StudentId).Distinct().Count(),
                        PresentCount = g.Count(a => a.IsPresent),
                        AbsentCount = g.Count(a => !a.IsPresent),
                        AttendanceRate = (double)g.Count(a => a.IsPresent) / g.Count() * 100
                    })
                    .ToList()
            };

            return report;
        }
    }
}