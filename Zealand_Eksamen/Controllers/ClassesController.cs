using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Data;
using Zealand_Eksamen.Models;


namespace Zealand_Eksamen.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    
    public class ClassesController :  ControllerBase
    {
    private readonly ApplicationDbContext _context;
    public ClassesController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // Get: api/Classes
    [HttpGet]
    
    public async Task<ActionResult<IEnumerable<Class>>> GetClasses()
    {
    return await _context.Classes.ToListAsync();
    }
    
    //Get: api/Classes/5
    [HttpGet("{id}")]
    
    public async Task<IActionResult> GetClass(int id)
    {
    var @class = await _context.Classes.FindAsync(id);
    if (@class == null)
    {
    return NotFound();
    }
    return Ok(@class);
    }
    
    // POST: api/Classes
    [HttpPost]
    
    public async Task<ActionResult<Class>> PostClass(Class @class)
    {
    _context.Classes.Add(@class);
    await _context.SaveChangesAsync();
    return CreatedAtAction(nameof(GetClass), new { id = @class.ClassID }, @class);
    }
    
    // PUT: api/Classes/5
    [HttpPut("{id}")]
    
    public async Task<IActionResult> PutClass(int id, Class @class)
    {
    if (id != @class.ClassID)
    {
        return BadRequest();
    }

    _context.Entry(@class).State = EntityState.Modified;
    try
    {
        await _context.SaveChangesAsync();

    }
    catch (DbUpdateConcurrencyException)
    {
        if (!ClassExists(id))
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
    
    // DELETE: api/Classes/5
    [HttpDelete("{id}")]
    
    public async Task<IActionResult> DeleteClass (int id)
    {
        var @class = await _context.Classes.FindAsync(id);
    if (@class == null)
    {
        return NotFound();
    }
    _context.Classes.Remove(@class);
    await _context.SaveChangesAsync();
    return NoContent();
    }
    private bool ClassExists(int id)
    {
        return _context.Classes.Any(e => e.ClassID == id);
    }
    }
}