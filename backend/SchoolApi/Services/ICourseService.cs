using SchoolApi.Models.DTOs;

/// <summary>
/// Service interface for managing courses, teacher assignments, and student enrollments.
/// </summary>
public interface ICourseService
{
    /// <summary>
    /// Get all courses.
    /// </summary>
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
    /// <summary>
    /// Create a new course.
    /// </summary>
    Task<CourseDto> CreateCourseAsync(CourseDto dto);
    /// <summary>
    /// Update an existing course.
    /// </summary>
    Task UpdateCourseAsync(Guid id, CourseDto dto);
    /// <summary>
    /// Delete a course.
    /// </summary>
    Task DeleteCourseAsync(Guid id);
    /// <summary>
    /// Assign a teacher to a course.
    /// </summary>
    Task AssignTeacherAsync(Guid courseId, Guid teacherId);
    /// <summary>
    /// Enroll a student in a course.
    /// </summary>
    Task EnrollStudentAsync(Guid courseId, Guid studentId);
    /// <summary>
    /// Get courses assigned to a teacher.
    /// </summary>
    Task<IEnumerable<CourseDto>> GetCoursesByTeacherAsync(Guid teacherId);
    /// <summary>
    /// Get courses a student is enrolled in.
    /// </summary>
    Task<IEnumerable<CourseDto>> GetCoursesByStudentAsync(Guid studentId);
}
