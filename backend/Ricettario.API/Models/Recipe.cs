namespace Ricettario.API.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public double PrepTimeMinutes { get; set; }
    public double CookTimeMinutes { get; set; }
    public string? Difficulty { get; set; } // Easy, Medium, Hard
    public string? ImageUrl { get; set; }
    
    // For Baker's Math
    public double TotalFlourWeight { get; set; }
    
    // Serving Info
    public int ServingPieces { get; set; }
    public double? PieceWeight { get; set; }

    // Category (Many-to-One)
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public List<Ingredient> Ingredients { get; set; } = new();
    
    // Phases & Videos
    public List<RecipePhase> Phases { get; set; } = new();
    public List<RecipeVideo> Videos { get; set; } = new();
    
    // Tags (Many-to-Many)
    public List<RecipeTag> RecipeTags { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
