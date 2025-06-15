using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("students")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<SearchResult<StudentSearchResult>>> SearchStudents(
            [FromQuery] string? query,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int? grade,
            [FromQuery] string? course,
            [FromQuery] string? teacher)
        {
            var students = _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .ThenInclude(c => c.Teacher)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                students = students.Where(s =>
                    s.FirstName.Contains(query) ||
                    s.LastName.Contains(query) ||
                    s.Email.Contains(query));
            }

            if (grade.HasValue)
            {
                students = students.Where(s => s.Grade == grade.Value);
            }

            if (!string.IsNullOrWhiteSpace(course))
            {
                students = students.Where(s => s.StudentCourses.Any(sc => sc.Course.Name.Contains(course)));
            }

            if (!string.IsNullOrWhiteSpace(teacher))
            {
                students = students.Where(s => s.StudentCourses.Any(sc =>
                    sc.Course.Teacher != null &&
                    (sc.Course.Teacher.FirstName.Contains(teacher) || sc.Course.Teacher.LastName.Contains(teacher))));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                students = sortBy.ToLower() switch
                {
                    "firstname" => sortOrder?.ToLower() == "desc"
                        ? students.OrderByDescending(s => s.FirstName)
                        : students.OrderBy(s => s.FirstName),
                    "lastname" => sortOrder?.ToLower() == "desc"
                        ? students.OrderByDescending(s => s.LastName)
                        : students.OrderBy(s => s.LastName),
                    "grade" => sortOrder?.ToLower() == "desc"
                        ? students.OrderByDescending(s => s.Grade)
                        : students.OrderBy(s => s.Grade),
                    _ => students
                };
            }

            var results = await students.Select(s => new StudentSearchResult
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Grade = s.Grade,
                Courses = s.StudentCourses.Select(sc => new CourseInfo
                {
                    Id = sc.Course.Id,
                    Title = sc.Course.Name,
                    TeacherName = sc.Course.Teacher != null
                        ? $"{sc.Course.Teacher.FirstName} {sc.Course.Teacher.LastName}"
                        : "Unassigned"
                }).ToList()
            }).ToListAsync();

            return new SearchResult<StudentSearchResult>
            {
                TotalCount = results.Count,
                Items = results
            };
        }

        [HttpGet("courses")]
        [Authorize]
        public async Task<ActionResult<SearchResult<CourseSearchResult>>> SearchCourses(
            [FromQuery] string? query,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] string? teacher,
            [FromQuery] bool? hasAttendance)
        {
            var courses = _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Attendances)
                .Include(c => c.StudentCourses)
                .ThenInclude(sc => sc.Student)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                courses = courses.Where(c =>
                    c.Name.Contains(query) ||
                    c.Description.Contains(query));
            }

            if (!string.IsNullOrWhiteSpace(teacher))
            {
                courses = courses.Where(c =>
                    c.Teacher != null &&
                    (c.Teacher.FirstName.Contains(teacher) ||
                    c.Teacher.LastName.Contains(teacher)));
            }

            if (hasAttendance.HasValue)
            {
                courses = courses.Where(c => c.Attendances.Any() == hasAttendance.Value);
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                courses = sortBy.ToLower() switch
                {
                    "title" => sortOrder?.ToLower() == "desc"
                        ? courses.OrderByDescending(c => c.Name)
                        : courses.OrderBy(c => c.Name),
                    "teacher" => sortOrder?.ToLower() == "desc"
                        ? courses.OrderByDescending(c => c.Teacher != null ? c.Teacher.LastName : string.Empty)
                        : courses.OrderBy(c => c.Teacher != null ? c.Teacher.LastName : string.Empty),
                    "students" => sortOrder?.ToLower() == "desc"
                        ? courses.OrderByDescending(c => c.StudentCourses.Count)
                        : courses.OrderBy(c => c.StudentCourses.Count),
                    _ => courses
                };
            }

            var results = await courses.Select(c => new CourseSearchResult
            {
                Id = c.Id,
                Title = c.Name,
                Description = c.Description,
                TeacherName = c.Teacher != null
                    ? $"{c.Teacher.FirstName} {c.Teacher.LastName}"
                    : "Unassigned",
                StudentCount = c.StudentCourses.Count,
                HasAttendance = c.Attendances.Any()
            }).ToListAsync();

            return new SearchResult<CourseSearchResult>
            {
                TotalCount = results.Count,
                Items = results
            };
        }
    }
}