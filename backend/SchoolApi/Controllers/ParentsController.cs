using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Parents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parent>>> GetParents()
        {
            return await _context.Parents
                .Include(p => p.User)
                .Include(p => p.Students)
                .ToListAsync();
        }

        // GET: api/Parents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Parent>> GetParent(int id)
        {
            var parent = await _context.Parents
                .Include(p => p.User)
                .Include(p => p.Students)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parent == null)
            {
                return NotFound();
            }

            return parent;
        }

        // POST: api/Parents
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Parent>> CreateParent(Parent parent)
        {
            _context.Parents.Add(parent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetParent), new { id = parent.Id }, parent);
        }

        // PUT: api/Parents/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateParent(int id, Parent parent)
        {
            if (id != parent.Id)
            {
                return BadRequest();
            }

            _context.Entry(parent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Parents/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteParent(int id)
        {
            var parent = await _context.Parents.FindAsync(id);
            if (parent == null)
            {
                return NotFound();
            }

            _context.Parents.Remove(parent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/students/{studentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddStudent(int id, int studentId)
        {
            var parent = await _context.Parents
                .Include(p => p.Students)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parent == null)
            {
                return NotFound("Parent not found");
            }

            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            parent.Students.Add(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}/students/{studentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveStudent(int id, int studentId)
        {
            var parent = await _context.Parents
                .Include(p => p.Students)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parent == null)
            {
                return NotFound("Parent not found");
            }

            var student = parent.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            parent.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParentExists(int id)
        {
            return _context.Parents.Any(e => e.Id == id);
        }
    }
}
