using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Data;
using Zealand_Eksamen.Models;

namespace Zealand_Eksamen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExamTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ExamTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamType>>> GetExamTypes()
        {
            return await _context.ExamTypes.ToListAsync();
        }

        // GET: api/ExamTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamType>> GetExamType(int id)
        {
            var examType = await _context.ExamTypes.FindAsync(id);

            if (examType == null)
            {
                return NotFound();
            }

            return examType;
        }

        // POST: api/ExamTypes
        [HttpPost]
        public async Task<ActionResult<ExamType>> PostExamType(ExamType examType)
        {
            _context.ExamTypes.Add(examType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExamType), new { id = examType.ExamTypeID }, examType);
        }

        // PUT: api/ExamTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamType(int id, ExamType examType)
        {
            if (id != examType.ExamTypeID)
            {
                return BadRequest();
            }

            _context.Entry(examType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamTypeExists(id))
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

        // DELETE: api/ExamTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamType(int id)
        {
            var examType = await _context.ExamTypes.FindAsync(id);
            if (examType == null)
            {
                return NotFound();
            }

            _context.ExamTypes.Remove(examType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamTypeExists(int id)
        {
            return _context.ExamTypes.Any(e => e.ExamTypeID == id);
        }
    }
}

