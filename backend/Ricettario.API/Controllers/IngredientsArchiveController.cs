using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ricettario.API.Data;
using Ricettario.API.Models;

namespace Ricettario.API.Controllers;

[Route("api/ingredients-archive")]
[ApiController]
public class IngredientsArchiveController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public IngredientsArchiveController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/ingredients-archive
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredientArchiveDto>>> GetIngredients(
        [FromQuery] IngredientCategory? category = null,
        [FromQuery] string? search = null)
    {
        var query = _context.IngredientArchives.AsQueryable();

        if (category.HasValue)
            query = query.Where(i => i.Category == category.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(i => i.Name.ToLower().Contains(search.ToLower()));

        // Pre-fetch conversion counts
        var conversionCounts = await _context.IngredientConversions
            .GroupBy(c => c.FromIngredientId)
            .Select(g => new { IngredientId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.IngredientId, x => x.Count);

        var reverseConversionCounts = await _context.IngredientConversions
            .GroupBy(c => c.ToIngredientId)
            .Select(g => new { IngredientId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.IngredientId, x => x.Count);

        var ingredientEntities = await query
            .OrderBy(i => i.Category)
            .ThenBy(i => i.SortOrder)
            .ThenBy(i => i.Name)
            .ToListAsync();

        var ingredients = ingredientEntities.Select(i => new IngredientArchiveDto
        {
            Id = i.Id,
            Name = i.Name,
            Description = i.Description,
            DefaultUnit = i.DefaultUnit,
            Category = i.Category,
            CategoryName = i.Category.ToString(),
            Icon = i.Icon,
            Color = i.Color,
            CustomProperties = i.GetCustomProperties(),
            IsSystemDefault = i.IsSystemDefault,
            SortOrder = i.SortOrder,
            ConversionCount = (conversionCounts.GetValueOrDefault(i.Id, 0) + reverseConversionCounts.GetValueOrDefault(i.Id, 0))
        }).ToList();

        return Ok(ingredients);
    }

    // GET: api/ingredients-archive/5
    [HttpGet("{id}")]
    public async Task<ActionResult<IngredientArchiveDto>> GetIngredient(int id)
    {
        var ingredient = await _context.IngredientArchives.FindAsync(id);

        if (ingredient == null)
            return NotFound();

        return Ok(MapToDto(ingredient));
    }

    // GET: api/ingredients-archive/5/conversions
    [HttpGet("{id}/conversions")]
    public async Task<ActionResult<IEnumerable<IngredientConversionDto>>> GetConversions(int id)
    {
        var ingredient = await _context.IngredientArchives.FindAsync(id);
        if (ingredient == null)
            return NotFound();

        // Get explicit conversions FROM this ingredient
        var conversionsFrom = await _context.IngredientConversions
            .Where(c => c.FromIngredientId == id)
            .Include(c => c.ToIngredient)
            .Select(c => new IngredientConversionDto
            {
                Id = c.Id,
                FromIngredientId = c.FromIngredientId,
                ToIngredientId = c.ToIngredientId,
                ToIngredientName = c.ToIngredient!.Name,
                ToIngredientIcon = c.ToIngredient.Icon,
                ToIngredientColor = c.ToIngredient.Color,
                ConversionRatio = c.ConversionRatio,
                Notes = c.Notes,
                IsReverse = false
            })
            .ToListAsync();

        // Get conversions TO this ingredient (reverse - bidirectional)
        var conversionsTo = await _context.IngredientConversions
            .Where(c => c.ToIngredientId == id)
            .Include(c => c.FromIngredient)
            .Select(c => new IngredientConversionDto
            {
                Id = c.Id,
                FromIngredientId = c.ToIngredientId, // Swap for reverse
                ToIngredientId = c.FromIngredientId,
                ToIngredientName = c.FromIngredient!.Name,
                ToIngredientIcon = c.FromIngredient.Icon,
                ToIngredientColor = c.FromIngredient.Color,
                ConversionRatio = c.ConversionRatio > 0 ? 1.0 / c.ConversionRatio : 1.0, // Inverse ratio
                Notes = c.Notes,
                IsReverse = true
            })
            .ToListAsync();

        // Merge and remove duplicates (prefer explicit over reverse)
        var explicitTargets = conversionsFrom.Select(c => c.ToIngredientId).ToHashSet();
        var allConversions = conversionsFrom
            .Concat(conversionsTo.Where(c => !explicitTargets.Contains(c.ToIngredientId)))
            .ToList();

        return Ok(allConversions);
    }

    // POST: api/ingredients-archive
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<IngredientArchiveDto>> CreateIngredient(IngredientArchiveCreateDto dto)
    {
        // Check for duplicate name
        if (await _context.IngredientArchives.AnyAsync(i => i.Name.ToLower() == dto.Name.ToLower()))
            return Conflict(new { message = "Un ingrediente con questo nome esiste già" });

        var ingredient = new IngredientArchive
        {
            Name = dto.Name,
            Description = dto.Description,
            DefaultUnit = dto.DefaultUnit ?? "g",
            Category = dto.Category,
            Icon = dto.Icon,
            Color = dto.Color,
            IsSystemDefault = false,
            SortOrder = dto.SortOrder
        };

        if (dto.CustomProperties != null && dto.CustomProperties.Count > 0)
            ingredient.SetCustomProperties(dto.CustomProperties);

        _context.IngredientArchives.Add(ingredient);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id }, MapToDto(ingredient));
    }

    // PUT: api/ingredients-archive/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateIngredient(int id, IngredientArchiveUpdateDto dto)
    {
        var ingredient = await _context.IngredientArchives.FindAsync(id);
        if (ingredient == null)
            return NotFound();

        // Check for duplicate name (excluding current)
        if (await _context.IngredientArchives.AnyAsync(i => i.Id != id && i.Name.ToLower() == dto.Name.ToLower()))
            return Conflict(new { message = "Un ingrediente con questo nome esiste già" });

        ingredient.Name = dto.Name;
        ingredient.Description = dto.Description;
        ingredient.DefaultUnit = dto.DefaultUnit ?? "g";
        ingredient.Category = dto.Category;
        ingredient.Icon = dto.Icon;
        ingredient.Color = dto.Color;
        ingredient.SortOrder = dto.SortOrder;

        if (dto.CustomProperties != null)
            ingredient.SetCustomProperties(dto.CustomProperties);
        else
            ingredient.CustomPropertiesJson = null;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/ingredients-archive/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        var ingredient = await _context.IngredientArchives.FindAsync(id);
        if (ingredient == null)
            return NotFound();

        if (ingredient.IsSystemDefault)
            return BadRequest(new { message = "Gli ingredienti di sistema non possono essere eliminati" });

        // Check if used in recipes
        var usedCount = await _context.Ingredients.CountAsync(i => i.ArchiveIngredientId == id);
        if (usedCount > 0)
            return BadRequest(new { message = $"Questo ingrediente è usato in {usedCount} ricette. Rimuovi prima i collegamenti." });

        // Remove related conversions
        var conversions = await _context.IngredientConversions
            .Where(c => c.FromIngredientId == id || c.ToIngredientId == id)
            .ToListAsync();
        _context.IngredientConversions.RemoveRange(conversions);

        _context.IngredientArchives.Remove(ingredient);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/ingredients-archive/5/conversions
    [HttpPost("{id}/conversions")]
    [Authorize]
    public async Task<ActionResult<IngredientConversionDto>> AddConversion(int id, IngredientConversionCreateDto dto)
    {
        var fromIngredient = await _context.IngredientArchives.FindAsync(id);
        if (fromIngredient == null)
            return NotFound(new { message = "Ingrediente sorgente non trovato" });

        var toIngredient = await _context.IngredientArchives.FindAsync(dto.ToIngredientId);
        if (toIngredient == null)
            return NotFound(new { message = "Ingrediente destinazione non trovato" });

        if (id == dto.ToIngredientId)
            return BadRequest(new { message = "Non puoi creare una conversione verso lo stesso ingrediente" });

        // Check if conversion already exists (either direction)
        var exists = await _context.IngredientConversions.AnyAsync(c =>
            (c.FromIngredientId == id && c.ToIngredientId == dto.ToIngredientId) ||
            (c.FromIngredientId == dto.ToIngredientId && c.ToIngredientId == id));

        if (exists)
            return Conflict(new { message = "Una conversione tra questi ingredienti esiste già" });

        var conversion = new IngredientConversion
        {
            FromIngredientId = id,
            ToIngredientId = dto.ToIngredientId,
            ConversionRatio = dto.ConversionRatio,
            Notes = dto.Notes
        };

        _context.IngredientConversions.Add(conversion);
        await _context.SaveChangesAsync();

        return Ok(new IngredientConversionDto
        {
            Id = conversion.Id,
            FromIngredientId = conversion.FromIngredientId,
            ToIngredientId = conversion.ToIngredientId,
            ToIngredientName = toIngredient.Name,
            ToIngredientIcon = toIngredient.Icon,
            ToIngredientColor = toIngredient.Color,
            ConversionRatio = conversion.ConversionRatio,
            Notes = conversion.Notes,
            IsReverse = false
        });
    }

    // DELETE: api/ingredients-archive/conversions/5
    [HttpDelete("conversions/{conversionId}")]
    [Authorize]
    public async Task<IActionResult> DeleteConversion(int conversionId)
    {
        var conversion = await _context.IngredientConversions.FindAsync(conversionId);
        if (conversion == null)
            return NotFound();

        _context.IngredientConversions.Remove(conversion);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/ingredients-archive/conversions/5
    [HttpPut("conversions/{conversionId}")]
    [Authorize]
    public async Task<IActionResult> UpdateConversion(int conversionId, IngredientConversionUpdateDto dto)
    {
        var conversion = await _context.IngredientConversions.FindAsync(conversionId);
        if (conversion == null)
            return NotFound();

        conversion.ConversionRatio = dto.ConversionRatio;
        conversion.Notes = dto.Notes;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/ingredients-archive/categories
    [HttpGet("categories")]
    public ActionResult<IEnumerable<object>> GetCategories()
    {
        var categories = Enum.GetValues<IngredientCategory>()
            .Select(c => new { value = (int)c, name = c.ToString() })
            .OrderBy(c => c.value)
            .ToList();

        return Ok(categories);
    }

    // Helper: Map entity to DTO
    private static IngredientArchiveDto MapToDto(IngredientArchive i) => new()
    {
        Id = i.Id,
        Name = i.Name,
        Description = i.Description,
        DefaultUnit = i.DefaultUnit,
        Category = i.Category,
        CategoryName = i.Category.ToString(),
        Icon = i.Icon,
        Color = i.Color,
        CustomProperties = i.GetCustomProperties(),
        IsSystemDefault = i.IsSystemDefault,
        SortOrder = i.SortOrder
    };
}

// ============================================================================
// DTOs
// ============================================================================

public class IngredientArchiveDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DefaultUnit { get; set; } = "g";
    public IngredientCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public List<IngredientCustomProperty> CustomProperties { get; set; } = new();
    public bool IsSystemDefault { get; set; }
    public int SortOrder { get; set; }
    public int ConversionCount { get; set; }
}

public class IngredientArchiveCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultUnit { get; set; }
    public IngredientCategory Category { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public List<IngredientCustomProperty>? CustomProperties { get; set; }
    public int SortOrder { get; set; }
}

public class IngredientArchiveUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultUnit { get; set; }
    public IngredientCategory Category { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public List<IngredientCustomProperty>? CustomProperties { get; set; }
    public int SortOrder { get; set; }
}

public class IngredientConversionDto
{
    public int Id { get; set; }
    public int FromIngredientId { get; set; }
    public int ToIngredientId { get; set; }
    public string ToIngredientName { get; set; } = string.Empty;
    public string? ToIngredientIcon { get; set; }
    public string? ToIngredientColor { get; set; }
    public double ConversionRatio { get; set; }
    public string? Notes { get; set; }
    public bool IsReverse { get; set; }
}

public class IngredientConversionCreateDto
{
    public int ToIngredientId { get; set; }
    public double ConversionRatio { get; set; }
    public string? Notes { get; set; }
}

public class IngredientConversionUpdateDto
{
    public double ConversionRatio { get; set; }
    public string? Notes { get; set; }
}
