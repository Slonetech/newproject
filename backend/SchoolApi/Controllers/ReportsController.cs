using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs.Reports; // Assuming these DTOs exist
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                        .ThenInclude(c => c.TeacherCourses) // Include TeacherCourses to get teacher info for courses
                            .ThenInclude(tc => tc.Teacher)
                .Where(s => s.IsActive) // Only active students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound("Student not found or is inactive.");
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
                        // Get all teachers for the course
                        Teachers = g.Key.TeacherCourses
                                         .Select(tc => $"{tc.Teacher.FirstName} {tc.Teacher.LastName}")
                                         .ToList(),
                        // Corrected lines: Assign directly to properties within the initializer
                        AverageGrade = g.Any() ? (decimal)g.Average(grade => grade.Value) : 0M, // Use 0M for decimal literal
                        LastGrade = g.Any() ? (decimal?)g.OrderByDescending(grade => grade.Date).FirstOrDefault()?.Value : null, // Handle nullable and potential null grade
                        LastGradeDate = g.OrderByDescending(grade => grade.Date).FirstOrDefault()?.Date ?? DateTime.MinValue // Null check
                    })
                    .ToList()
            };

            return report;
        }

        [HttpGet("parent/{id}")]
        public async Task<ActionResult<IEnumerable<StudentProgressReport>>> GetParentReport(Guid id)
        {
            var parent = await _context.Parents
                .Include(p => p.ChildLinks)
                    .ThenInclude(cl => cl.Student)
                        .ThenInclude(s => s.Grades)
                            .ThenInclude(g => g.Course)
                                .ThenInclude(c => c.TeacherCourses)
                                    .ThenInclude(tc => tc.Teacher)
                .Where(p => p.IsActive) // Only active parents
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parent == null)
            {
                return NotFound("Parent not found or is inactive.");
            }

            var reports = parent.Students.Where(s => s.IsActive).Select(student => new StudentProgressReport
            {
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                Courses = student.Grades
                    .GroupBy(g => g.Course)
                    .Select(g => new CourseProgress
                    {
                        CourseId = g.Key.Id,
                        CourseName = g.Key.Name,
                        Teachers = g.Key.TeacherCourses
                                         .Select(tc => $"{tc.Teacher.FirstName} {tc.Teacher.LastName}")
                                         .ToList(),
                        // Corrected lines: Assign directly to properties within the initializer
                        AverageGrade = g.Any() ? (decimal)g.Average(grade => grade.Value) : 0M,
                        LastGrade = g.Any() ? (decimal?)g.OrderByDescending(grade => grade.Date).FirstOrDefault()?.Value : null,
                        LastGradeDate = g.OrderByDescending(grade => grade.Date).FirstOrDefault()?.Date ?? DateTime.MinValue
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
                .Include(c => c.TeacherCourses) // Include TeacherCourses to get teacher info for course summary
                    .ThenInclude(tc => tc.Teacher)
                .Include(c => c.Enrollments)
                .Where(c => c.IsActive) // Only active courses
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound("Course not found or is inactive.");
            }

            var grades = course.Grades.Select(g => g.Value).ToList();
            var report = new CourseSummaryReport
            {
                CourseId = course.Id,
                CourseName = course.Name,
                CourseCode = course.Code, // Include Course Code
                Teachers = course.TeacherCourses.Select(tc => $"{tc.Teacher.FirstName} {tc.Teacher.LastName}").ToList(), // Get all assigned teachers
                TotalStudents = course.Enrollments.Count(e => e.Student.IsActive), // Count active students
                AverageGrade = grades.Any() ? (decimal)grades.Average() : 0M, // Explicit cast to decimal
                HighestGrade = grades.Any() ? (decimal)grades.Max() : 0M,    // Explicit cast to decimal
                LowestGrade = grades.Any() ? (decimal)grades.Min() : 0M,      // Explicit cast to decimal
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
                        CourseId = g.Key?.Id ?? Guid.Empty, // Null check for g.Key
                        CourseName = g.Key?.Name ?? "Unknown Course", // Null check for g.Key
                        TotalStudents = g.Select(a => a.StudentId).Distinct().Count(),
                        PresentCount = g.Count(a => a.IsPresent),
                        AbsentCount = g.Count(a => !a.IsPresent),
                        // Avoid division by zero
                        AttendanceRate = g.Any() ? (double)g.Count(a => a.IsPresent) / g.Count() * 100 : 0
                    })
                    .ToList()
            };

            return report;
        }
    }
}