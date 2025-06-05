using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Models;

namespace SchoolApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Parent> Parents { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Grade> Grades { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<StudentCourse> StudentCourses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    builder.Entity<Attendance>()
        .Property(a => a.Status)
        .HasConversion<int>();

    builder.Entity<StudentCourse>()
        .HasKey(sc => new { sc.StudentId, sc.CourseId });

    builder.Entity<StudentCourse>()
        .HasOne(sc => sc.Student)
        .WithMany(s => s.StudentCourses)
        .HasForeignKey(sc => sc.StudentId);

    builder.Entity<StudentCourse>()
        .HasOne(sc => sc.Course)
        .WithMany(c => c.StudentCourses)
        .HasForeignKey(sc => sc.CourseId);

    builder.Entity<Parent>()
        .HasOne(p => p.User)
        .WithOne(u => u.Parent)
        .HasForeignKey<Parent>(p => p.UserId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Student>()
        .HasOne(s => s.User)
        .WithOne(u => u.Student)
        .HasForeignKey<Student>(s => s.UserId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Teacher>()
        .HasOne(t => t.User)
        .WithOne(u => u.Teacher)
        .HasForeignKey<Teacher>(t => t.UserId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Grade>()
        .HasOne(g => g.Student)
        .WithMany(s => s.Grades)
        .HasForeignKey(g => g.StudentId);

    builder.Entity<Attendance>()
        .HasOne(a => a.Student)
        .WithMany(s => s.Attendances)
        .HasForeignKey(a => a.StudentId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Entity<Attendance>()
        .HasOne(a => a.Teacher)
        .WithMany(t => t.Attendances)
        .HasForeignKey(a => a.TeacherId)
        .OnDelete(DeleteBehavior.Restrict);

    // 🔥 Add this
    builder.Entity<Student>()
        .HasOne(s => s.Parent)
        .WithMany(p => p.Students)
        .HasForeignKey(s => s.ParentId)
        .OnDelete(DeleteBehavior.Restrict);
}

    }
}
