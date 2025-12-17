using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Data;
using Zealand_Eksamen.Models;

namespace Zealand_Eksamen.Controllers
{
public class ClassesPageController : Controller
{
private readonly ApplicationDbContext _context;
public ClassesPageController(ApplicationDbContext context)
{
_context = context;
}
public IActionResult Index()
{
    var items = new List<Class>
    {
        new Class { ClassID = 1, ClassCode = "DAT1A" },
        new Class { ClassID = 2, ClassCode = "DAT1B" },
        new Class { ClassID = 3, ClassCode = "DAT2A" }
    };

    return View(items);
}
                
public IActionResult Create()
{
return View(new Class());
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Class model)
{
if (!ModelState.IsValid) return View(model);
_context.Classes.Add(model);
await _context.SaveChangesAsync();
return RedirectToAction(nameof(Index));
}
public async Task<IActionResult> Edit(int id)
{
var entity = await _context.Classes.FindAsync(id);
    if (entity == null) return NotFound();
    return View(entity);
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Class model)
{
if (id != model.ClassID) return BadRequest();
if (!ModelState.IsValid) return View(model);
_context.Entry(model).State = EntityState.Modified;
await _context.SaveChangesAsync();
return RedirectToAction(nameof(Index));
}
public async Task<IActionResult> Delete(int id)
{
    var entity = await _context.Classes.FindAsync(id);
    if (entity == null) return NotFound();
    return View(entity);
}
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{ 
    var entity = await _context.Classes.FindAsync(id);
    if (entity != null)
    {
        _context.Classes.Remove(entity);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction(nameof(Index));
}
}
}


