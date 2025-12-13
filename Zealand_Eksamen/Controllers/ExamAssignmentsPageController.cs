using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Data;
using Zealand_Eksamen.Models;

namespace Zealand_Eksamen.Controllers
{
    public class ExamAssignmentsPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamAssignmentsPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List assignments for an exam
        public async Task<IActionResult> ByExam(int examId)
        {
            var exam = await _context.Exams
                .Include(e => e.Class)
                .FirstOrDefaultAsync(e => e.ExamID == examId);
            if (exam == null) return NotFound();

            var assignments = await _context.ExamAssignments
                .Include(a => a.Employee)
                .Where(a => a.ExamID == examId)
                .ToListAsync();

            ViewBag.Exam = exam;
            return View(assignments);
        }

        // Create assignment for an exam
        public async Task<IActionResult> Create(int examId)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam == null) return NotFound();
            ViewBag.Exam = exam;
            ViewBag.Employees = new SelectList(await _context.Employees.AsNoTracking().ToListAsync(), "EmployeeID", "FullName");
            return View(new ExamAssignment { ExamID = examId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamAssignment model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Exam = await _context.Exams.FindAsync(model.ExamID);
                ViewBag.Employees = new SelectList(await _context.Employees.AsNoTracking().ToListAsync(), "EmployeeID", "FullName");
                return View(model);
            }

            // Overlap validation for Employee on ordinary exam day(s)
            var exam = await _context.Exams.AsNoTracking().FirstOrDefaultAsync(e => e.ExamID == model.ExamID);
            if (exam?.OrdinaryStartDate != null)
            {
                var start = exam.OrdinaryStartDate.Value.Date;
                var end = (exam.OrdinaryEndDate ?? exam.OrdinaryStartDate)!.Value.Date;

                var conflict = await _context.ExamAssignments
                    .Include(ea => ea.Exam)
                    .AsNoTracking()
                    .Where(ea => ea.EmployeeID == model.EmployeeID && ea.ExamID != model.ExamID)
                    .AnyAsync(ea => ea.Exam.OrdinaryStartDate != null &&
                                    ea.Exam.OrdinaryStartDate!.Value.Date <= end &&
                                    (ea.Exam.OrdinaryEndDate ?? ea.Exam.OrdinaryStartDate)!.Value.Date >= start);

                if (conflict)
                {
                    ModelState.AddModelError("", "Denne medarbejder har allerede en ordinær eksamen på den valgte dag(e).");
                    ViewBag.Exam = await _context.Exams.FindAsync(model.ExamID);
                    ViewBag.Employees = new SelectList(await _context.Employees.AsNoTracking().ToListAsync(), "EmployeeID", "FullName");
                    return View(model);
                }
            }

            _context.ExamAssignments.Add(model);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "En tildeling findes allerede for denne eksamen og medarbejder.");
                ViewBag.Exam = await _context.Exams.FindAsync(model.ExamID);
                ViewBag.Employees = new SelectList(await _context.Employees.AsNoTracking().ToListAsync(), "EmployeeID", "FullName");
                return View(model);
            }
            return RedirectToAction(nameof(ByExam), new { examId = model.ExamID });
        }

        // Delete assignment
        public async Task<IActionResult> Delete(int id)
        {
            var assignment = await _context.ExamAssignments
                .Include(a => a.Employee)
                .Include(a => a.Exam)
                .FirstOrDefaultAsync(a => a.ExamAssignmentID == id);
            if (assignment == null) return NotFound();
            return View(assignment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.ExamAssignments.FindAsync(id);
            if (assignment != null)
            {
                var examId = assignment.ExamID;
                _context.ExamAssignments.Remove(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ByExam), new { examId });
            }
            return RedirectToAction("Index", "ExamsPage");
        }
    }
}
