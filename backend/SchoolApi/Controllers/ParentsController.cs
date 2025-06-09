using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;

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
        public async Task<ActionResult<IEnumerable<Parent>>> GetParents()
        {
            return await _context.Parents
                                 .Include(p => p.User) // Include linked Identity User
                                 .Include(p => p.Children) // Include linked Student children
                                 .ToListAsync();
        }

        // GET: api/Parents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Parent>> GetParent(int id)
        {
            var parent = await _context.Parents
                                       .Include(p => p.User)
                                       .Include(p => p.Children)
                                       .FirstOrDefaultAsync(p => p.ParentId == id); // Corrected to ParentId

            if (parent == null)
            {
                return NotFound();
            }

            return parent;
        }

        // PUT: api/Parents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParent(int id, Parent parent)
        {
            if (id != parent.ParentId) // Corrected to ParentId
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

        // POST: api/Parents
        [HttpPost]
        public async Task<ActionResult<Parent>> PostParent(Parent parent)
        {
            _context.Parents.Add(parent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParent", new { id = parent.ParentId }, parent); // Corrected to ParentId
        }

        // DELETE: api/Parents/5
        [HttpDelete("{id}")]
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

        private bool ParentExists(int id)
        {
            return _context.Parents.Any(e => e.ParentId == id); // Corrected to ParentId
        }
    }
}
