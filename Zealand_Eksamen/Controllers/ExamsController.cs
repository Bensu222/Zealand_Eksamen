using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Data;
using Zealand_Eksamen.Models;

namespace Zealand_Eksamen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Exams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exam>>> GetExams()
        {
            return await _context.Exams
                .Include(e => e.Class)
                .Include(e => e.ExamType)
                .ToListAsync();
        }

        // GET: api/Exams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Exam>> GetExam(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.Class)
                .Include(e => e.ExamType)
                .FirstOrDefaultAsync(e => e.ExamID == id);

            if (exam == null)
            {
                return NotFound();
            }

            return exam;
        }

        // POST: api/Exams
        [HttpPost]
        public async Task<ActionResult<Exam>> PostExam(Exam exam)
        {
            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExam), new { id = exam.ExamID }, exam);
        }

        // PUT: api/Exams/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExam(int id, Exam exam)
        {
            if (id != exam.ExamID)
            {
                return BadRequest();
            }

            _context.Entry(exam).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamExists(id))
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

        // DELETE: api/Exams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.ExamID == id);
        }
    }
}
