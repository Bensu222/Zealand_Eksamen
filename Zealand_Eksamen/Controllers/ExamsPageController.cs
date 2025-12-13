using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Data;
using Zealand_Eksamen.Models;

namespace Zealand_Eksamen.Controllers
{
    public class ExamsPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamsPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q, string? classCode, DateTime? from, DateTime? to)
        {
            var query = _context.Exams
                .Include(e => e.Class)
                .Include(e => e.ExamType)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(e => e.ExamName.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(classCode))
            {
                var cterm = classCode.Trim();
                query = query.Where(e => e.Class.ClassCode.Contains(cterm));
            }

            if (from != null)
            {
                var f = from.Value.Date;
                query = query.Where(e => e.OrdinaryStartDate != null && (e.OrdinaryEndDate ?? e.OrdinaryStartDate)!.Value.Date >= f);
            }

            if (to != null)
            {
                var t = to.Value.Date;
                query = query.Where(e => e.OrdinaryStartDate != null && e.OrdinaryStartDate!.Value.Date <= t);
            }

            ViewBag.Query = q;
            ViewBag.ClassCode = classCode;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            var items = await query
                .OrderBy(e => e.OrdinaryStartDate)
                .ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateSelectLists();
            ViewBag.HasClasses = await _context.Classes.AnyAsync();
            ViewBag.HasExamTypes = await _context.ExamTypes.AnyAsync();
            return View(new Exam());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectLists();
                ViewBag.HasClasses = await _context.Classes.AnyAsync();
                ViewBag.HasExamTypes = await _context.ExamTypes.AnyAsync();
                return View(model);
            }

            // Overlap validation for Class on ordinary exam dates
            if (await HasClassOverlap(model))
            {
                ModelState.AddModelError("", "Dette hold har allerede en ordinær eksamen på den valgte dag(e).");
                await PopulateSelectLists();
                ViewBag.HasClasses = await _context.Classes.AnyAsync();
                ViewBag.HasExamTypes = await _context.ExamTypes.AnyAsync();
                return View(model);
            }

            try
            {
                _context.Exams.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Fejl ved lagring af eksamen: {ex.Message}");
                await PopulateSelectLists();
                ViewBag.HasClasses = await _context.Classes.AnyAsync();
                ViewBag.HasExamTypes = await _context.ExamTypes.AnyAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.Exams.FindAsync(id);
            if (entity == null) return NotFound();
            await PopulateSelectLists();
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exam model)
        {
            if (id != model.ExamID) return BadRequest();
            if (!ModelState.IsValid)
            {
                await PopulateSelectLists();
                return View(model);
            }

            if (await HasClassOverlap(model, excludeExamId: id))
            {
                ModelState.AddModelError("", "Dette hold har allerede en ordinær eksamen på den valgte dag(e).");
                await PopulateSelectLists();
                return View(model);
            }
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Exams
                .Include(e => e.Class)
                .Include(e => e.ExamType)
                .FirstOrDefaultAsync(e => e.ExamID == id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.Exams.FindAsync(id);
            if (entity != null)
            {
                _context.Exams.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateSelectLists()
        {
            ViewBag.Classes = new SelectList(await _context.Classes.AsNoTracking().ToListAsync(), "ClassID", "ClassCode");
            ViewBag.ExamTypes = new SelectList(await _context.ExamTypes.AsNoTracking().ToListAsync(), "ExamTypeID", "TypeName");
        }

        private async Task<bool> HasClassOverlap(Exam model, int? excludeExamId = null)
        {
            if (model.ClassID == null || model.OrdinaryStartDate == null) return false;
            var start = model.OrdinaryStartDate!.Value.Date;
            var end = (model.OrdinaryEndDate ?? model.OrdinaryStartDate)!.Value.Date;

            var query = _context.Exams.AsNoTracking()
                .Where(e => e.ClassID == model.ClassID && e.OrdinaryStartDate != null)
                .Where(e => (excludeExamId == null || e.ExamID != excludeExamId.Value));

            // Overlap: (existing.start <= new.end) && (new.start <= existing.end)
            return await query.AnyAsync(e =>
                e.OrdinaryStartDate!.Value.Date <= end &&
                (e.OrdinaryEndDate ?? e.OrdinaryStartDate)!.Value.Date >= start
            );
        }
    }
}



