using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Data;
using Zealand_Eksamen.Models;

namespace Zealand_Eksamen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamAssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExamAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ExamAssignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamAssignment>>> GetExamAssignments()
        {
            return await _context.ExamAssignments
                .Include(ea => ea.Exam)
                .Include(ea => ea.Employee)
                .ToListAsync();
        }

        // GET: api/ExamAssignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamAssignment>> GetExamAssignment(int id)
        {
            var examAssignment = await _context.ExamAssignments
                .Include(ea => ea.Exam)
                .Include(ea => ea.Employee)
                .FirstOrDefaultAsync(ea => ea.ExamAssignmentID == id);

            if (examAssignment == null)
            {
                return NotFound();
            }

            return examAssignment;
        }

        // POST: api/ExamAssignments
        [HttpPost]
        public async Task<ActionResult<ExamAssignment>> PostExamAssignment(ExamAssignment examAssignment)
        {
            _context.ExamAssignments.Add(examAssignment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ExamAssignmentExists(examAssignment.ExamID, examAssignment.EmployeeID))
                {
                    return Conflict("An assignment already exists for this exam and employee combination.");
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetExamAssignment), new { id = examAssignment.ExamAssignmentID }, examAssignment);
        }

        // PUT: api/ExamAssignments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamAssignment(int id, ExamAssignment examAssignment)
        {
            if (id != examAssignment.ExamAssignmentID)
            {
                return BadRequest();
            }

            _context.Entry(examAssignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamAssignmentExistsById(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                if (ExamAssignmentExists(examAssignment.ExamID, examAssignment.EmployeeID, id))
                {
                    return Conflict("An assignment already exists for this exam and employee combination.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ExamAssignments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamAssignment(int id)
        {
            var examAssignment = await _context.ExamAssignments.FindAsync(id);
            if (examAssignment == null)
            {
                return NotFound();
            }

            _context.ExamAssignments.Remove(examAssignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamAssignmentExistsById(int id)
        {
            return _context.ExamAssignments.Any(ea => ea.ExamAssignmentID == id);
        }

        private bool ExamAssignmentExists(int examId, int employeeId)
        {
            return _context.ExamAssignments.Any(ea => ea.ExamID == examId && ea.EmployeeID == employeeId);
        }

        private bool ExamAssignmentExists(int examId, int employeeId, int excludeId)
        {
            return _context.ExamAssignments.Any(ea => ea.ExamID == examId && ea.EmployeeID == employeeId && ea.ExamAssignmentID != excludeId);
        }
    }
    
}
