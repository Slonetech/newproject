using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs;

namespace SchoolApi.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;
        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Select(c => new CourseDto { Id = c.Id, Name = c.Name, Description = c.Description, Credits = c.Credits })
                .ToListAsync();
        }

        public async Task<CourseDto> CreateCourseAsync(CourseDto dto)
        {
            var course = new Course { Name = dto.Name, Description = dto.Description, Credits = dto.Credits };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            dto.Id = course.Id;
            return dto;
        }

        public async Task UpdateCourseAsync(Guid id, CourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) throw new KeyNotFoundException("Course not found");
            course.Name = dto.Name;
            course.Description = dto.Description;
            course.Credits = dto.Credits;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) throw new KeyNotFoundException("Course not found");
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task AssignTeacherAsync(Guid courseId, Guid teacherId)
        {
            var exists = await _context.Set<TeacherCourse>().AnyAsync(tc => tc.CourseId == courseId && tc.TeacherId == teacherId);
            if (!exists)
            {
                _context.Set<TeacherCourse>().Add(new TeacherCourse { CourseId = courseId, TeacherId = teacherId });
                await _context.SaveChangesAsync();
            }
        }

        public async Task EnrollStudentAsync(Guid courseId, Guid studentId)
        {
            var exists = await _context.Enrollments.AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);
            if (!exists)
            {
                _context.Enrollments.Add(new Enrollment { CourseId = courseId, StudentId = studentId, EnrolledAt = System.DateTime.UtcNow });
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByTeacherAsync(Guid teacherId)
        {
            return await _context.TeacherCourses
                .Where(tc => tc.TeacherId == teacherId)
                .Select(tc => new CourseDto { Id = tc.Course.Id, Name = tc.Course.Name, Description = tc.Course.Description, Credits = tc.Course.Credits })
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByStudentAsync(Guid studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => new CourseDto { Id = e.Course.Id, Name = e.Course.Name, Description = e.Course.Description, Credits = e.Course.Credits })
                .ToListAsync();
        }
    }
}