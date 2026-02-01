namespace Ricettario.API.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#0d6efd"; // Hex color
    public int UsageCount { get; set; } = 0; // Denormalized for quick sorting by popularity
    
    // Navigation - Many-to-Many with Recipe
    public List<RecipeTag> RecipeTags { get; set; } = new();
}

// Join table for Recipe-Tag many-to-many
public class RecipeTag
{
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    
    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
