using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs.Search; // Assuming these DTOs exist
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
            [FromQuery] string? teacher) // This will now search for a teacher associated with a course
        {
            var students = _context.Students
                .Include(s => s.StudentCourses)
                    .ThenInclude(sc => sc.Course)
                        .ThenInclude(c => c.TeacherCourses) // Include TeacherCourses for the course
                            .ThenInclude(tc => tc.Teacher) // Then include the Teacher
                .Where(s => s.IsActive) // Only active students
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                students = students.Where(s =>
                    s.FirstName.Contains(query) ||
                    s.LastName.Contains(query) ||
                    s.Email.Contains(query) ||
                    s.Address.Contains(query) ||
                    s.PhoneNumber.Contains(query)
                );
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
                    sc.Course.TeacherCourses.Any(tc =>
                        tc.Teacher.FirstName.Contains(teacher) || tc.Teacher.LastName.Contains(teacher)
                    )
                ));
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
                    // Now, get teacher names from the TeacherCourses collection
                    TeacherNames = sc.Course.TeacherCourses
                                        .Select(tc => $"{tc.Teacher.FirstName} {tc.Teacher.LastName}")
                                        .ToList()
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
                .Include(c => c.TeacherCourses) // Include the join table
                    .ThenInclude(tc => tc.Teacher) // Then the Teacher
                .Include(c => c.Attendances)
                .Include(c => c.StudentCourses)
                    .ThenInclude(sc => sc.Student)
                .Where(c => c.IsActive) // Only active courses
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                courses = courses.Where(c =>
                    c.Name.Contains(query) ||
                    c.Code.Contains(query) || // Added Code to search
                    c.Description.Contains(query)
                );
            }

            if (!string.IsNullOrWhiteSpace(teacher))
            {
                // Filter by teacher name through the TeacherCourses join table
                courses = courses.Where(c =>
                    c.TeacherCourses.Any(tc =>
                        tc.Teacher.FirstName.Contains(teacher) || tc.Teacher.LastName.Contains(teacher)
                    )
                );
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
                    "code" => sortOrder?.ToLower() == "desc" // Added sort by code
                        ? courses.OrderByDescending(c => c.Code)
                        : courses.OrderBy(c => c.Code),
                    "teacher" => sortOrder?.ToLower() == "desc"
                        ? courses.OrderByDescending(c => c.TeacherCourses.Any() ? c.TeacherCourses.Select(tc => tc.Teacher.LastName).FirstOrDefault() : string.Empty)
                        : courses.OrderBy(c => c.TeacherCourses.Any() ? c.TeacherCourses.Select(tc => tc.Teacher.LastName).FirstOrDefault() : string.Empty),
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
                Code = c.Code, // Added Code to search result
                Description = c.Description,
                // Combine all assigned teacher names
                TeacherNames = c.TeacherCourses
                                .Select(tc => $"{tc.Teacher.FirstName} {tc.Teacher.LastName}")
                                .ToList(),
                StudentCount = c.StudentCourses.Count,
                HasAttendance = c.Attendances.Any()
            }).ToListAsync();

            return new SearchResult<CourseSearchResult>
            {
                TotalCount = results.Count,
                Items = results
            };
        }

        // You might want to add other search methods (e.g., for Teachers, Parents, etc.)
        // Ensure you handle Includes and property access correctly for any related entities.
        [HttpGet("teachers")]
        [Authorize(Roles = "Admin,Student")] // Example roles for accessing teacher info
        public async Task<ActionResult<SearchResult<TeacherSearchResult>>> SearchTeachers(
            [FromQuery] string? query,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder)
        {
            var teachers = _context.Teachers
                .Include(t => t.TeacherCourses) // Include for courses taught
                    .ThenInclude(tc => tc.Course)
                .Where(t => t.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                teachers = teachers.Where(t =>
                    t.FirstName.Contains(query) ||
                    t.LastName.Contains(query) ||
                    t.Email.Contains(query) ||
                    t.Specialization!.Contains(query) ||
                    t.Department!.Contains(query) ||
                    t.TeacherCourses.Any(tc => tc.Course.Name.Contains(query)) // Search by course name taught
                );
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                teachers = sortBy.ToLower() switch
                {
                    "firstname" => sortOrder?.ToLower() == "desc"
                        ? teachers.OrderByDescending(t => t.FirstName)
                        : teachers.OrderBy(t => t.FirstName),
                    "lastname" => sortOrder?.ToLower() == "desc"
                        ? teachers.OrderByDescending(t => t.LastName)
                        : teachers.OrderBy(t => t.LastName),
                    "specialization" => sortOrder?.ToLower() == "desc"
                        ? teachers.OrderByDescending(t => t.Specialization)
                        : teachers.OrderBy(t => t.Specialization),
                    _ => teachers
                };
            }

            var results = await teachers.Select(t => new TeacherSearchResult // Assuming TeacherSearchResult DTO exists
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Email = t.Email,
                Specialization = t.Specialization,
                Department = t.Department,
                CoursesTaught = t.TeacherCourses.Select(tc => new CourseInfo
                {
                    Id = tc.Course.Id,
                    Title = tc.Course.Name,
                    Code = tc.Course.Code // Include code
                }).ToList()
            }).ToListAsync();

            return new SearchResult<TeacherSearchResult>
            {
                TotalCount = results.Count,
                Items = results
            };
        }
    }
}