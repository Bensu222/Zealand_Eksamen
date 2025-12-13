using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Data;
using Zealand_Eksamen.Models;

namespace Zealand_Eksamen.Controllers
{
    public partial class EmployeesPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.Employees.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(e => e.FullName.Contains(term) || e.Email.Contains(term));
            }
            ViewBag.Query = q;
            var items = await query.ToListAsync();
            return View(items);
        }

        public IActionResult Create()
        {
            return View(new Employee());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee model)
        {
            if (!ModelState.IsValid) return View(model);
            _context.Employees.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.Employees.FindAsync(id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        private IActionResult View(object viewName)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee model)
        {
            if (id != model.EmployeeID) return BadRequest();
            if (!ModelState.IsValid) return View(model);
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Employees.FindAsync(id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.Employees.FindAsync(id);
            if (entity != null)
            {
                _context.Employees.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
