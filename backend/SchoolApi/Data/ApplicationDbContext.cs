using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SchoolApi.Models; // Ensure your models are accessible here

namespace SchoolApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<TeacherCourse> TeacherCourses { get; set; } // Add this DbSet
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure cascade delete behavior
            builder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne()
                .HasForeignKey<Teacher>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Parent>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<Parent>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Parent-Student relationship
            builder.Entity<Student>()
                .HasOne(s => s.Parent)
                .WithMany(p => p.Students)
                .HasForeignKey(s => s.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure many-to-many relationships for StudentCourse
            builder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            builder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure many-to-many relationship for TeacherCourse
            builder.Entity<TeacherCourse>()
                .HasKey(tc => new { tc.TeacherId, tc.CourseId }); // Define composite primary key

            builder.Entity<TeacherCourse>()
                .HasOne(tc => tc.Teacher)
                .WithMany(t => t.TeacherCourses)
                .HasForeignKey(tc => tc.TeacherId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust cascade behavior as needed

            builder.Entity<TeacherCourse>()
                .HasOne(tc => tc.Course)
                .WithMany(c => c.TeacherCourses)
                .HasForeignKey(tc => tc.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust cascade behavior as needed

            // Removed the old Course-Teacher one-to-many relationship
            // builder.Entity<Course>()
            //     .HasOne(c => c.Teacher)
            //     .WithMany(t => t.Courses)
            //     .HasForeignKey(c => c.TeacherId)
            //     .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Grade>()
                .HasOne(g => g.Course)
                .WithMany(c => c.Grades)
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Announcement and Event relationships
            builder.Entity<Announcement>()
                .HasOne(a => a.CreatedBy)
                .WithMany()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Event>()
                .HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure indexes
            builder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            builder.Entity<Teacher>()
                .HasIndex(t => t.Email)
                .IsUnique();

            builder.Entity<Parent>()
                .HasIndex(p => p.Email)
                .IsUnique();

            builder.Entity<Course>()
                .HasIndex(c => c.Code)
                .IsUnique();

            // Seed basic course data
            builder.Entity<Course>().HasData(
                new Course
                {
                    Id = Guid.NewGuid(),
                    Name = "Mathematics",
                    Code = "MATH101",
                    Description = "Introduction to basic mathematics concepts",
                    Credits = 3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Id = Guid.NewGuid(),
                    Name = "Science",
                    Code = "SCI101",
                    Description = "Introduction to scientific principles",
                    Credits = 3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Id = Guid.NewGuid(),
                    Name = "English Language",
                    Code = "ENG101",
                    Description = "English language and literature",
                    Credits = 3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Id = Guid.NewGuid(),
                    Name = "Social Studies",
                    Code = "SOC101",
                    Description = "Introduction to social sciences",
                    Credits = 3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Configure warnings to handle dynamic values
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }
}