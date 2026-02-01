using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ricettario.API.Data;
using Ricettario.API.Models;

namespace Ricettario.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TagsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/tags
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTags([FromQuery] string? search, [FromQuery] int limit = 50)
    {
        var query = _context.Tags.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Name.Contains(search));

        var tags = await query
            .OrderByDescending(t => t.UsageCount)
            .ThenBy(t => t.Name)
            .Take(limit)
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
                UsageCount = t.UsageCount
            })
            .ToListAsync();

        return Ok(tags);
    }

    // GET: api/tags/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(int id)
    {
        var tag = await _context.Tags
            .Where(t => t.Id == id)
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
                UsageCount = t.UsageCount
            })
            .FirstOrDefaultAsync();

        if (tag == null)
            return NotFound();

        return Ok(tag);
    }

    // POST: api/tags
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TagDto>> CreateTag([FromBody] TagCreateDto dto)
    {
        // Check if tag with same name exists (case insensitive)
        var existing = await _context.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == dto.Name.ToLower());

        if (existing != null)
            return Ok(new TagDto
            {
                Id = existing.Id,
                Name = existing.Name,
                Color = existing.Color,
                UsageCount = existing.UsageCount
            });

        var tag = new Tag
        {
            Name = dto.Name.Trim(),
            Color = dto.Color ?? GenerateRandomColor(),
            UsageCount = 0
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Color = tag.Color,
            UsageCount = tag.UsageCount
        });
    }

    // PUT: api/tags/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] TagUpdateDto dto)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
            return NotFound();

        tag.Name = dto.Name ?? tag.Name;
        tag.Color = dto.Color ?? tag.Color;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/tags/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var tag = await _context.Tags
            .Include(t => t.RecipeTags)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tag == null)
            return NotFound();

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/tags/batch - Create multiple tags at once
    [HttpPost("batch")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TagDto>>> CreateTagsBatch([FromBody] List<string> tagNames)
    {
        var result = new List<TagDto>();

        foreach (var name in tagNames.Where(n => !string.IsNullOrWhiteSpace(n)))
        {
            var trimmedName = name.Trim();
            var existing = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == trimmedName.ToLower());

            if (existing != null)
            {
                result.Add(new TagDto
                {
                    Id = existing.Id,
                    Name = existing.Name,
                    Color = existing.Color,
                    UsageCount = existing.UsageCount
                });
            }
            else
            {
                var tag = new Tag
                {
                    Name = trimmedName,
                    Color = GenerateRandomColor(),
                    UsageCount = 0
                };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();

                result.Add(new TagDto
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Color = tag.Color,
                    UsageCount = tag.UsageCount
                });
            }
        }

        return Ok(result);
    }

    // GET: api/tags/popular
    [HttpGet("popular")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetPopularTags([FromQuery] int limit = 20)
    {
        var tags = await _context.Tags
            .OrderByDescending(t => t.UsageCount)
            .Take(limit)
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
                UsageCount = t.UsageCount
            })
            .ToListAsync();

        return Ok(tags);
    }

    private static string GenerateRandomColor()
    {
        var colors = new[]
        {
            "#0d6efd", "#6610f2", "#6f42c1", "#d63384", "#dc3545",
            "#fd7e14", "#ffc107", "#198754", "#20c997", "#0dcaf0",
            "#6c757d", "#495057"
        };
        return colors[Random.Shared.Next(colors.Length)];
    }
}

// DTOs
public record TagDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public int UsageCount { get; init; }
}

public record TagCreateDto
{
    public string Name { get; init; } = string.Empty;
    public string? Color { get; init; }
}

public record TagUpdateDto
{
    public string? Name { get; init; }
    public string? Color { get; init; }
}
