namespace Ricettario.API.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    
    /// <summary>
    /// Baker's Percentage relative to the total flour in the recipe.
    /// </summary>
    public double BakersPercentage { get; set; }
    
    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
    
    /// <summary>
    /// Optional link to an archived ingredient for conversions and properties.
    /// Null for legacy ingredients (backward compatibility).
    /// </summary>
    public int? ArchiveIngredientId { get; set; }
    public IngredientArchive? ArchiveIngredient { get; set; }
}
