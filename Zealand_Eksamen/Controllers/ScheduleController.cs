using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Data;

namespace Zealand_Eksamen.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // All employees with their upcoming exams
        public async Task<IActionResult> Employees()
        {
            var assignmentsByEmployee = await _context.Employees
                .Include(emp => emp.ExamAssignments)
                    .ThenInclude(ea => ea.Exam)
                        .ThenInclude(e => e.Class)
                .AsNoTracking()
                .ToListAsync();

            // sort each employee's assignments by date
            foreach (var emp in assignmentsByEmployee)
            {
                emp.ExamAssignments = emp.ExamAssignments
                    .OrderBy(a => a.Exam.OrdinaryStartDate)
                    .ToList();
            }

            return View(assignmentsByEmployee);
        }

        // All classes with their exams
        public async Task<IActionResult> Classes()
        {
            var classes = await _context.Classes
                .Include(c => c.Exams)
                .ThenInclude(e => e.ExamType)
                .AsNoTracking()
                .ToListAsync();

            foreach (var c in classes)
            {
                c.Exams = c.Exams
                    .OrderBy(e => e.OrdinaryStartDate)
                    .ToList();
            }

            return View(classes);
        }

        public async Task<IActionResult> Employee(int id)
        {
            var assignments = await _context.ExamAssignments
                .Include(ea => ea.Exam)
                .ThenInclude(e => e.Class)
                .Where(ea => ea.EmployeeID == id)
                .OrderBy(ea => ea.Exam.OrdinaryStartDate)
                .ToListAsync();

            ViewBag.EmployeeId = id;
            var employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.EmployeeID == id);
            ViewBag.EmployeeName = employee?.FullName;
            ViewBag.Overlaps = DetectOverlaps(assignments.Select(a => a.Exam).ToList());
            return View(assignments);
        }

        public async Task<IActionResult> Class(int id)
        {
            var exams = await _context.Exams
                .Where(e => e.ClassID == id)
                .OrderBy(e => e.OrdinaryStartDate)
                .ToListAsync();

            ViewBag.ClassId = id;
            ViewBag.Overlaps = DetectOverlaps(exams);
            return View(exams);
        }

        private static List<(DateOnly day, int count)> DetectOverlaps(List<Models.Exam> exams)
        {
            var dayCounts = new Dictionary<DateOnly, int>();
            foreach (var exam in exams)
            {
                if (exam.OrdinaryStartDate.HasValue)
                {
                    var day = DateOnly.FromDateTime(exam.OrdinaryStartDate.Value.Date);
                    dayCounts[day] = dayCounts.TryGetValue(day, out var c) ? c + 1 : 1;
                }
            }
            return dayCounts.Where(kv => kv.Value > 1).Select(kv => (kv.Key, kv.Value)).ToList();
        }
    }
}
