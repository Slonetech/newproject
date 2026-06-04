namespace SchoolApi.Models.Enums
{
    /// <summary>
    /// Represents the attendance status of a student
    /// </summary>
    public enum AttendanceStatus
    {
        /// <summary>
        /// Student was present
        /// </summary>
        Present = 0,

        /// <summary>
        /// Student was absent without justification
        /// </summary>
        Absent = 1,

        /// <summary>
        /// Student arrived late
        /// </summary>
        Late = 2,

        /// <summary>
        /// Student was absent with valid excuse
        /// </summary>
        Excused = 3
    }
}
