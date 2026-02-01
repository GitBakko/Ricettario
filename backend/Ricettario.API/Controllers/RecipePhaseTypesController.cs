using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ricettario.API.Data;
using Ricettario.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace Ricettario.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipePhaseTypesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RecipePhaseTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/RecipePhaseTypes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipePhaseType>>> GetRecipePhaseTypes()
    {
        return await _context.RecipePhaseTypes.ToListAsync();
    }

    // GET: api/RecipePhaseTypes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RecipePhaseType>> GetRecipePhaseType(int id)
    {
        var recipePhaseType = await _context.RecipePhaseTypes.FindAsync(id);

        if (recipePhaseType == null)
        {
            return NotFound();
        }

        return recipePhaseType;
    }

    // PUT: api/RecipePhaseTypes/5
    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin")] // Uncomment if you want restriction
    public async Task<IActionResult> PutRecipePhaseType(int id, RecipePhaseType recipePhaseType)
    {
        if (id != recipePhaseType.Id)
        {
            return BadRequest();
        }

        if (recipePhaseType.IsSystemDefault) 
        {
             // Prevent editing system defaults completely? Or just ID? 
             // Logic: Allow editing name/active but maybe warn? 
             // For now allow edits.
        }

        _context.Entry(recipePhaseType).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RecipePhaseTypeExists(id))
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

    // POST: api/RecipePhaseTypes
    [HttpPost]
    public async Task<ActionResult<RecipePhaseType>> PostRecipePhaseType(RecipePhaseType recipePhaseType)
    {
        recipePhaseType.Id = 0; // Ensure creation
        recipePhaseType.IsSystemDefault = false; // User created
        _context.RecipePhaseTypes.Add(recipePhaseType);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetRecipePhaseType", new { id = recipePhaseType.Id }, recipePhaseType);
    }

    // DELETE: api/RecipePhaseTypes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipePhaseType(int id)
    {
        var recipePhaseType = await _context.RecipePhaseTypes.FindAsync(id);
        if (recipePhaseType == null)
        {
            return NotFound();
        }

        if (recipePhaseType.IsSystemDefault)
        {
             return BadRequest("Cannot delete system default types.");
        }

        _context.RecipePhaseTypes.Remove(recipePhaseType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RecipePhaseTypeExists(int id)
    {
        return _context.RecipePhaseTypes.Any(e => e.Id == id);
    }
}
