// File: C:\Users\Slonetech\Documents\agile\newproject\backend\SchoolApi\Models\DTOs\Search\CourseInfo.cs

namespace SchoolApi.Models.DTOs.Search
{
    public class CourseInfo
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // From your SearchController, it looks like CourseInfo has a Code property
        public List<string> TeacherNames { get; set; } = new List<string>(); // If CourseInfo is reused for StudentSearchResult's courses
    }
}