using System;
using System.Collections.Generic;

namespace SchoolApi.Models.DTOs.Reports
{
    public class AttendanceSummaryReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public List<CourseAttendance> CourseBreakdown { get; set; } = new();
    }

    public class CourseAttendance
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double AttendanceRate { get; set; }
    }
}