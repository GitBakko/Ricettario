using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ricettario.API.Data;
using Ricettario.API.DTOs;
using Ricettario.API.Models;
using Ricettario.API.Services;

namespace Ricettario.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IBakersPercentageService _bakersPercentageService;
    private readonly IPdfReportingService _pdfReportingService;
    private readonly IWebHostEnvironment _environment;

    public RecipesController(
        ApplicationDbContext context, 
        IBakersPercentageService bakersPercentageService,
        IPdfReportingService pdfReportingService,
        IWebHostEnvironment environment)
    {
        _context = context;
        _bakersPercentageService = bakersPercentageService;
        _pdfReportingService = pdfReportingService;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetRecipes(
        [FromQuery] int? categoryId = null,
        [FromQuery] string? tags = null,
        [FromQuery] string? search = null)
    {
        var query = _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Category)
            .Include(r => r.RecipeTags)
                .ThenInclude(rt => rt.Tag)
            .AsQueryable();

        // Filter by category
        if (categoryId.HasValue)
            query = query.Where(r => r.CategoryId == categoryId.Value);

        // Filter by tags (comma-separated tag IDs)
        if (!string.IsNullOrWhiteSpace(tags))
        {
            var tagIds = tags.Split(',').Select(int.Parse).ToList();
            query = query.Where(r => r.RecipeTags.Any(rt => tagIds.Contains(rt.TagId)));
        }

        // Search in title/description
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r => r.Title.Contains(search) || (r.Description != null && r.Description.Contains(search)));
        }

        var recipes = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.ImageUrl,
                r.Difficulty,
                r.PrepTimeMinutes,
                r.CookTimeMinutes,
                r.ServingPieces,
                r.PieceWeight,
                r.TotalFlourWeight,
                r.CreatedAt,
                Category = r.Category == null ? null : new { r.Category.Id, r.Category.Name, r.Category.Icon, r.Category.Color },
                Tags = r.RecipeTags.Select(rt => new { rt.Tag.Id, rt.Tag.Name, rt.Tag.Color }).ToList(),
                Ingredients = r.Ingredients
            })
            .ToListAsync();

        return Ok(recipes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetRecipe(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Category)
            .Include(r => r.RecipeTags)
                .ThenInclude(rt => rt.Tag)
            .Include(r => r.Phases)
                .ThenInclude(p => p.PhaseIngredients)
            .Include(r => r.Phases)
                .ThenInclude(p => p.RecipePhaseType)
            .Include(r => r.Videos)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
        {
            return NotFound();
        }

        // Return with category and tags properly formatted
        return Ok(new
        {
            recipe.Id,
            recipe.Title,
            recipe.Description,
            recipe.Instructions,
            recipe.ImageUrl,
            recipe.Difficulty,
            recipe.PrepTimeMinutes,
            recipe.CookTimeMinutes,
            recipe.ServingPieces,
            recipe.PieceWeight,
            recipe.TotalFlourWeight,
            recipe.CreatedAt,
            recipe.UpdatedAt,
            recipe.CategoryId,
            Category = recipe.Category == null ? null : new { recipe.Category.Id, recipe.Category.Name, recipe.Category.Icon, recipe.Category.Color },
            Tags = recipe.RecipeTags.Select(rt => new { rt.Tag.Id, rt.Tag.Name, rt.Tag.Color }).ToList(),
            TagIds = recipe.RecipeTags.Select(rt => rt.TagId).ToList(),
            recipe.Ingredients,
            recipe.Phases,
            recipe.Videos
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Recipe>> CreateRecipe([FromBody] CreateRecipeDto dto)
    {
        var allPhaseTypes = await _context.RecipePhaseTypes.ToListAsync();

        // 0. Map DTO to Entity
        var recipe = new Recipe
        {
            Title = dto.Title,
            Description = dto.Description,
            Instructions = dto.Instructions,
            PrepTimeMinutes = dto.PrepTimeMinutes,
            CookTimeMinutes = dto.CookTimeMinutes,
            Difficulty = dto.Difficulty,
            ImageUrl = dto.ImageUrl,
            TotalFlourWeight = dto.TotalFlourWeight,
            ServingPieces = dto.ServingPieces,
            PieceWeight = dto.PieceWeight,
            CategoryId = dto.CategoryId,
            Ingredients = dto.Ingredients.Select(i => new Ingredient
            {
                Name = i.Name,
                Quantity = i.Quantity,
                Unit = i.Unit
            }).ToList(),
            Videos = dto.VideoUrls.Select(url => new RecipeVideo { Url = url }).ToList(),
            Phases = dto.Phases.Select(p => {
                return new RecipePhase
                {
                    Title = p.Title,
                    RecipePhaseTypeId = p.RecipePhaseTypeId,
                    Description = p.Description,
                    DurationMinutes = p.DurationMinutes,
                    Temperature = p.Temperature,
                    OvenMode = p.OvenMode,
                    PhaseIngredients = p.PhaseIngredients.Select(pi => new PhaseIngredient
                    {
                        IngredientName = pi.IngredientName,
                        Quantity = pi.Quantity,
                        Unit = pi.Unit
                    }).ToList()
                };
            }).ToList(),
            RecipeTags = dto.TagIds.Select(tagId => new RecipeTag { TagId = tagId }).ToList()
        };

        // Calculate Times
        double activeTime = 0;
        double passiveTime = 0;
        foreach(var phase in recipe.Phases)
        {
             var pt = allPhaseTypes.FirstOrDefault(t => t.Id == phase.RecipePhaseTypeId);
             if (pt != null)
             {
                 if (pt.IsActiveWork) activeTime += phase.DurationMinutes;
                 else passiveTime += phase.DurationMinutes;
             }
        }
        recipe.PrepTimeMinutes = activeTime;
        recipe.CookTimeMinutes = passiveTime;

        // 1. Calculate Math
        _bakersPercentageService.CalculatePercentages(recipe);
        
        // 2. Set User from Claims
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if(string.IsNullOrEmpty(userId)) return Unauthorized();
        
        recipe.UserId = userId; 
        
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateRecipe(int id, [FromBody] CreateRecipeDto dto)
    {
        var allPhaseTypes = await _context.RecipePhaseTypes.ToListAsync();

        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Videos)
            .Include(r => r.RecipeTags)
            .Include(r => r.Phases)
                .ThenInclude(p => p.PhaseIngredients)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (recipe.UserId != userId) return Forbid(); // Or Unauthorized

        // Update Fields
        recipe.Title = dto.Title;
        recipe.Description = dto.Description;
        recipe.Instructions = dto.Instructions;
        recipe.PrepTimeMinutes = dto.PrepTimeMinutes;
        recipe.CookTimeMinutes = dto.CookTimeMinutes;
        recipe.Difficulty = dto.Difficulty;
        recipe.ImageUrl = dto.ImageUrl;
        recipe.TotalFlourWeight = dto.TotalFlourWeight;
        recipe.ServingPieces = dto.ServingPieces;
        recipe.PieceWeight = dto.PieceWeight;
        recipe.CategoryId = dto.CategoryId;
        recipe.UpdatedAt = DateTime.UtcNow;

        // Update Collections: Clear and Re-add strategy
        _context.Ingredients.RemoveRange(recipe.Ingredients);
        _context.RecipeVideos.RemoveRange(recipe.Videos);
        _context.Set<RecipeTag>().RemoveRange(recipe.RecipeTags);
        // Phases need careful removal because of sub-collection
        foreach(var p in recipe.Phases)
        {
             _context.Set<PhaseIngredient>().RemoveRange(p.PhaseIngredients);
        }
        _context.RecipePhases.RemoveRange(recipe.Phases);

        // Add new collections
        recipe.Ingredients = dto.Ingredients.Select(i => new Ingredient
        {
            Name = i.Name,
            Quantity = i.Quantity,
            Unit = i.Unit,
            RecipeId = recipe.Id // Explicit linking though EF handles it
        }).ToList();

        recipe.Videos = dto.VideoUrls.Select(url => new RecipeVideo { Url = url, RecipeId = recipe.Id }).ToList();

        recipe.Phases = dto.Phases.Select(p => {
            return new RecipePhase
            {
                Title = p.Title,
                RecipePhaseTypeId = p.RecipePhaseTypeId,
                Description = p.Description,
                DurationMinutes = p.DurationMinutes,
                Temperature = p.Temperature,
                OvenMode = p.OvenMode,
                RecipeId = recipe.Id,
                PhaseIngredients = p.PhaseIngredients.Select(pi => new PhaseIngredient
                {
                    IngredientName = pi.IngredientName,
                    Quantity = pi.Quantity,
                    Unit = pi.Unit
                }).ToList()
            };
        }).ToList();

        // Update Tags
        recipe.RecipeTags = dto.TagIds.Select(tagId => new RecipeTag 
        { 
            RecipeId = recipe.Id,
            TagId = tagId 
        }).ToList();


        // Calculate Times
        double updActiveTime = 0;
        double updPassiveTime = 0;
        foreach(var phase in recipe.Phases)
        {
             var pt = allPhaseTypes.FirstOrDefault(t => t.Id == phase.RecipePhaseTypeId);
             if (pt != null)
             {
                 if (pt.IsActiveWork) updActiveTime += phase.DurationMinutes;
                 else updPassiveTime += phase.DurationMinutes;
             }
        }
        recipe.PrepTimeMinutes = updActiveTime;
        recipe.CookTimeMinutes = updPassiveTime;

        // Recalculate percentages
        _bakersPercentageService.CalculatePercentages(recipe);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Recipes.Any(e => e.Id == id)) return NotFound();
            else throw;
        }

        return NoContent();
    }

    [HttpPost("upload-image")]
    [Authorize]
    public async Task<ActionResult<object>> UploadImage(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "images", "recipes");
            
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var safeFileName = Path.GetFileName(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"{Request.Scheme}://{Request.Host}/images/recipes/{uniqueFileName}";
            return Ok(new { url });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message} - Stack: {ex.StackTrace}");
        }
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetRecipePdf(int id, [FromQuery] int? pieces, [FromQuery] double? pieceWeight, [FromQuery] double? totalFlour)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Phases)
                .ThenInclude(p => p.PhaseIngredients)
            .Include(r => r.Phases)
                .ThenInclude(p => p.RecipePhaseType)
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (recipe == null) return NotFound();

        // Check if resizing is needed
        if (pieces.HasValue && pieceWeight.HasValue && pieces.Value > 0 && pieceWeight.Value > 0)
        {
            double originalTotalWeight = recipe.Ingredients.Sum(i => i.Quantity);
            double targetTotalWeight = pieces.Value * pieceWeight.Value;
            double ratio = (originalTotalWeight > 0) ? targetTotalWeight / originalTotalWeight : 1;

            var newWeights = _bakersPercentageService.ResizeByPieces(recipe, pieces.Value, pieceWeight.Value);
            // Apply new weights to main ingredients
            foreach(var ing in recipe.Ingredients)
            {
                if (newWeights.ContainsKey(ing.Name))
                {
                    ing.Quantity = newWeights[ing.Name];
                }
            }
            
            // Apply ratio to Phase Ingredients
            if (Math.Abs(ratio - 1) > 0.0001) 
            {
                foreach(var phase in recipe.Phases)
                {
                    foreach(var pi in phase.PhaseIngredients)
                    {
                        pi.Quantity *= ratio;
                    }
                }
            }

            // Update metadata for PDF
            recipe.ServingPieces = pieces.Value;
            recipe.PieceWeight = pieceWeight.Value;
            // Recalculate total flour for display if needed
            recipe.TotalFlourWeight = recipe.Ingredients.Where(i => i.Name.Contains("Farina", StringComparison.OrdinalIgnoreCase)).Sum(i => i.Quantity);
        }
        else if (totalFlour.HasValue && totalFlour.Value > 0)
        {
            double ratio = (recipe.TotalFlourWeight > 0) ? totalFlour.Value / recipe.TotalFlourWeight : 1;
            
            var newWeights = _bakersPercentageService.ResizeByFlour(recipe, totalFlour.Value);
             foreach(var ing in recipe.Ingredients)
            {
                if (newWeights.ContainsKey(ing.Name))
                {
                    ing.Quantity = newWeights[ing.Name];
                }
            }
            
            // Apply ratio to Phase Ingredients
            if (Math.Abs(ratio - 1) > 0.0001) 
            {
                foreach(var phase in recipe.Phases)
                {
                    foreach(var pi in phase.PhaseIngredients)
                    {
                        pi.Quantity *= ratio;
                    }
                }
            }
            
            recipe.TotalFlourWeight = totalFlour.Value;
        }

        var pdfBytes = await _pdfReportingService.GenerateRecipePdf(recipe);
        
        return File(pdfBytes, "application/pdf", $"{recipe.Title.Replace(" ", "_")}.pdf");
    }

    [HttpPost("{id}/resize")]
    public async Task<IActionResult> ResizeRecipe(int id, [FromQuery] double? newFlour, [FromQuery] int? pieces, [FromQuery] double? pieceWeight)
    {
         var recipe = await _context.Recipes.Include(r => r.Ingredients).FirstOrDefaultAsync(r => r.Id == id);
         if (recipe == null) return NotFound();

         Dictionary<string, double>? result = null;

         if (newFlour.HasValue)
         {
             result = _bakersPercentageService.ResizeByFlour(recipe, newFlour.Value);
         }
         else if (pieces.HasValue && pieceWeight.HasValue)
         {
             result = _bakersPercentageService.ResizeByPieces(recipe, pieces.Value, pieceWeight.Value);
         }
         
         if (result == null) return BadRequest("Invalid parameters");

         return Ok(result);
    }

    /// <summary>
    /// Delete a recipe (only owner can delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Videos)
            .Include(r => r.RecipeTags)
            .Include(r => r.Phases)
                .ThenInclude(p => p.PhaseIngredients)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null) return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (recipe.UserId != userId) return Forbid();

        // Delete related image file if exists
        if (!string.IsNullOrEmpty(recipe.ImageUrl) && recipe.ImageUrl.StartsWith("/uploads/"))
        {
            var imagePath = Path.Combine(_environment.WebRootPath ?? "wwwroot", recipe.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        // Remove all related entities
        foreach (var phase in recipe.Phases)
        {
            _context.Set<PhaseIngredient>().RemoveRange(phase.PhaseIngredients);
        }
        _context.RecipePhases.RemoveRange(recipe.Phases);
        _context.Ingredients.RemoveRange(recipe.Ingredients);
        _context.RecipeVideos.RemoveRange(recipe.Videos);
        _context.Set<RecipeTag>().RemoveRange(recipe.RecipeTags);
        _context.Recipes.Remove(recipe);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
