using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeachersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Teachers
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTeachers()
        {
            var teachers = await _context.Teachers.ToListAsync();
            return Ok(teachers);
        }

        // GET: api/Teachers/profile
        [HttpGet("profile")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherProfile()
        {
            // Extract logged-in user's ID from JWT claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token." });

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
                return NotFound(new { message = "Teacher profile not found." });

            return Ok(teacher);
        }

        // GET: api/Teachers/me
        [HttpGet("me")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token." });

            var teacher = await _context.Teachers
                .Include(t => t.TeacherCourses)
                    .ThenInclude(tc => tc.Course)
                .FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
                return NotFound(new { message = "Teacher profile not found." });

            var dto = new
            {
                id = teacher.Id,
                firstName = teacher.FirstName,
                lastName = teacher.LastName,
                email = teacher.Email,
                courses = teacher.TeacherCourses.Select(tc => new
                {
                    id = tc.Course.Id,
                    name = tc.Course.Name,
                    code = tc.Course.Code
                }).ToList()
            };
            return Ok(dto);
        }

        // POST: api/Teachers
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTeacher([FromBody] Models.DTOs.TeacherCreateDto dto)
        {
            // Log the incoming request for debugging
            Console.WriteLine($"CreateTeacher called with dto: {System.Text.Json.JsonSerializer.Serialize(dto)}");

            if (dto == null)
            {
                Console.WriteLine("CreateTeacher: dto is null");
                return BadRequest(new { message = "Teacher data is required" });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                Console.WriteLine($"CreateTeacher: Validation failed with errors: {string.Join(", ", errors)}");
                return BadRequest(new { message = "Validation failed", errors });
            }

            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                Console.WriteLine($"CreateTeacher: Email {dto.Email} already exists");
                return Conflict(new { message = "A user with this email already exists." });
            }

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            // Get UserManager service
            var userManager = HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;
            if (userManager == null)
            {
                return StatusCode(500, new { message = "UserManager service not available." });
            }

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                Console.WriteLine($"CreateTeacher: Failed to create user. Errors: {string.Join(", ", errors)}");
                return BadRequest(new { message = "Failed to create user.", errors = result.Errors });
            }

            // Assign Teacher role
            var roleResult = await userManager.AddToRoleAsync(user, "Teacher");
            if (!roleResult.Succeeded)
            {
                // If role assignment fails, clean up the created user
                await userManager.DeleteAsync(user);
                return BadRequest(new { message = "Failed to assign Teacher role.", errors = roleResult.Errors });
            }

            Teacher teacher;
            try
            {
                // Create Teacher entity
                teacher = new Teacher
                {
                    UserId = user.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Specialization = dto.Specialization,
                    Department = dto.Department,
                    DateOfBirth = dto.DateOfBirth,
                    HireDate = dto.HireDate,
                    Address = dto.Address,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();

                // Link ApplicationUser to Teacher
                user.TeacherId = teacher.Id;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // If Teacher creation fails, clean up the created user
                await userManager.DeleteAsync(user);
                return StatusCode(500, new { message = "Failed to create teacher profile.", error = ex.Message });
            }

            return CreatedAtAction(nameof(GetAllTeachers), new { id = teacher.Id }, new { teacher.Id, teacher.FirstName, teacher.LastName, teacher.Email });
        }

        // PUT: api/Teachers/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTeacher(Guid id, [FromBody] Models.DTOs.TeacherCreateDto dto)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound(new { message = "Teacher not found." });

            teacher.FirstName = dto.FirstName;
            teacher.LastName = dto.LastName;
            teacher.Email = dto.Email;
            teacher.PhoneNumber = dto.PhoneNumber;
            teacher.Specialization = dto.Specialization;
            teacher.Department = dto.Department;
            teacher.DateOfBirth = dto.DateOfBirth;
            teacher.HireDate = dto.HireDate;
            teacher.Address = dto.Address;
            teacher.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Teacher updated successfully." });
        }
    }
}