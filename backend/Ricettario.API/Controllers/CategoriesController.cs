using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ricettario.API.Data;
using Ricettario.API.Models;

namespace Ricettario.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Icon = c.Icon,
                Color = c.Color,
                SortOrder = c.SortOrder,
                IsSystemDefault = c.IsSystemDefault,
                RecipeCount = c.Recipes.Count
            })
            .ToListAsync();

        return Ok(categories);
    }

    // GET: api/categories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Icon = c.Icon,
                Color = c.Color,
                SortOrder = c.SortOrder,
                IsSystemDefault = c.IsSystemDefault,
                RecipeCount = c.Recipes.Count
            })
            .FirstOrDefaultAsync();

        if (category == null)
            return NotFound();

        return Ok(category);
    }

    // POST: api/categories
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryCreateDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            Icon = dto.Icon ?? "bi-folder",
            Color = dto.Color ?? "#6c757d",
            SortOrder = dto.SortOrder ?? await _context.Categories.MaxAsync(c => (int?)c.SortOrder) + 1 ?? 1,
            IsSystemDefault = false
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Icon = category.Icon,
            Color = category.Color,
            SortOrder = category.SortOrder,
            IsSystemDefault = category.IsSystemDefault,
            RecipeCount = 0
        });
    }

    // PUT: api/categories/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        category.Name = dto.Name ?? category.Name;
        category.Description = dto.Description ?? category.Description;
        category.Icon = dto.Icon ?? category.Icon;
        category.Color = dto.Color ?? category.Color;
        category.SortOrder = dto.SortOrder ?? category.SortOrder;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/categories/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound();

        if (category.IsSystemDefault)
            return BadRequest("Cannot delete system default categories");

        // Set recipes in this category to null
        var recipes = await _context.Recipes.Where(r => r.CategoryId == id).ToListAsync();
        foreach (var recipe in recipes)
            recipe.CategoryId = null;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/categories/reorder
    [HttpPut("reorder")]
    [Authorize]
    public async Task<IActionResult> ReorderCategories([FromBody] List<CategoryReorderDto> items)
    {
        foreach (var item in items)
        {
            var category = await _context.Categories.FindAsync(item.Id);
            if (category != null)
                category.SortOrder = item.SortOrder;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}

// DTOs
public record CategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Icon { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public bool IsSystemDefault { get; init; }
    public int RecipeCount { get; init; }
}

public record CategoryCreateDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public int? SortOrder { get; init; }
}

public record CategoryUpdateDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public int? SortOrder { get; init; }
}

public record CategoryReorderDto
{
    public int Id { get; init; }
    public int SortOrder { get; init; }
}
