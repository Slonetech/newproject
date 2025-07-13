using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Parents
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllParents()
        {
            var parents = await _context.Parents.ToListAsync();
            return Ok(parents);
        }

        // GET: api/Parents/children
        [HttpGet("children")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> GetChildren()
        {
            // Extract logged-in user's ID from JWT claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token." });

            var parent = await _context.Parents
                .Include(p => p.ChildLinks)
                    .ThenInclude(cl => cl.Student)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (parent == null)
                return NotFound(new { message = "Parent profile not found." });

            var children = parent.ChildLinks.Select(cl => cl.Student).ToList();
            return Ok(children);
        }

        // GET: api/Parents/me
        [HttpGet("me")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token." });

            var parent = await _context.Parents
                .Include(p => p.ChildLinks)
                    .ThenInclude(cl => cl.Student)
                        .ThenInclude(s => s.Enrollments)
                            .ThenInclude(e => e.Course)
                .Include(p => p.ChildLinks)
                    .ThenInclude(cl => cl.Student)
                        .ThenInclude(s => s.Grades)
                            .ThenInclude(g => g.Course)
                .Include(p => p.ChildLinks)
                    .ThenInclude(cl => cl.Student)
                        .ThenInclude(s => s.Attendances)
                            .ThenInclude(a => a.Course)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (parent == null)
                return NotFound(new { message = "Parent profile not found." });

            var dto = new
            {
                id = parent.Id,
                firstName = parent.FirstName,
                lastName = parent.LastName,
                email = parent.Email,
                children = parent.ChildLinks.Select(cl =>
                {
                    var s = cl.Student;
                    return new
                    {
                        id = s.Id,
                        firstName = s.FirstName,
                        lastName = s.LastName,
                        courses = s.Enrollments.Select(e => new
                        {
                            id = e.Course.Id,
                            name = e.Course.Name,
                            code = e.Course.Code
                        }).ToList(),
                        grades = s.Grades.Select(g => new
                        {
                            id = g.Id,
                            courseName = g.Course.Name,
                            value = g.Value
                        }).ToList(),
                        attendance = s.Attendances.Select(a => new
                        {
                            id = a.Id,
                            courseName = a.Course.Name,
                            isPresent = a.IsPresent,
                            date = a.Date
                        }).ToList()
                    };
                }).ToList()
            };
            return Ok(dto);
        }
    }
}
