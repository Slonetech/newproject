// File: C:\Users\Slonetech\Documents\agile\newproject\backend\SchoolApi\Models\DTOs\Search\TeacherSearchResult.cs

using System.Collections.Generic;

namespace SchoolApi.Models.DTOs.Search
{
    public class TeacherSearchResult
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string? Department { get; set; }
        public List<CourseInfo> CoursesTaught { get; set; } = new List<CourseInfo>();
    }

    // You likely also have CourseInfo defined in this namespace or another one that's referenced.
    // If CourseInfo is in a different namespace, you'll need a using directive for it too.
    // For simplicity, I'm including it here as an example if it's not defined elsewhere.
    
}