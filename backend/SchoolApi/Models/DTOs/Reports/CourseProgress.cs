// File: C:\Users\Slonetech\Documents\agile\newproject\backend\SchoolApi\Models\DTOs\Reports\CourseProgress.cs

using System;
using System.Collections.Generic;

namespace SchoolApi.Models.DTOs.Reports
{
    public class CourseProgress
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty; // Changed from CourseTitle to CourseName
        public string CourseCode { get; set; } = string.Empty;
        public List<string> Teachers { get; set; } = new List<string>();

        // NEW PROPERTIES ADDED:
        public decimal AverageGrade { get; set; }
        public decimal? LastGrade { get; set; }
        public DateTime? LastGradeDate { get; set; }
        // End of NEW PROPERTIES

        public decimal CurrentGrade { get; set; } // This was already here
        public int CompletedAssignments { get; set; }
        public int TotalAssignments { get; set; }
        public decimal ProgressPercentage => TotalAssignments > 0 ? (decimal)CompletedAssignments / TotalAssignments * 100 : 0;
        public int Absences { get; set; }
        public int Lates { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}