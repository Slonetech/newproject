using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Models; // Ensure your models are accessible here

namespace SchoolApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Parent> Parents { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<TeacherCourse> TeacherCourses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call the base method for Identity models

            // Configure Grade.Score precision and scale
            modelBuilder.Entity<Grade>()
                .Property(g => g.Score)
                .HasPrecision(5, 2); // Example: 5 total digits, 2 after decimal (e.g., 999.99 or 100.00)

            // Configure Student-ApplicationUser relationship
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Student>(s => s.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Teacher-ApplicationUser relationship
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne()
                .HasForeignKey<Teacher>(t => t.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Parent-ApplicationUser relationship
            modelBuilder.Entity<Parent>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<Parent>(p => p.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Student-Parent relationship
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(s => s.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Many-to-Many: Student-Course via StudentCourse
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);

            // Configure Many-to-Many: Teacher-Course via TeacherCourse
            modelBuilder.Entity<TeacherCourse>()
                .HasKey(tc => new { tc.TeacherId, tc.CourseId });

            modelBuilder.Entity<TeacherCourse>()
                .HasOne(tc => tc.Teacher)
                .WithMany(t => t.TeacherCourses)
                .HasForeignKey(tc => tc.TeacherId);

            modelBuilder.Entity<TeacherCourse>()
                .HasOne(tc => tc.Course)
                .WithMany(c => c.TeacherCourses)
                .HasForeignKey(tc => tc.CourseId);

            // Configure Grade relationships
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Course)
                .WithMany(c => c.Grades)
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Teacher)
                .WithMany(t => t.AssignedGrades)
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Attendance relationships
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Teacher)
                .WithMany(t => t.MarkedAttendances)
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
