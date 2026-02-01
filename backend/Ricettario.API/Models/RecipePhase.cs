namespace Ricettario.API.Models;

public class RecipePhase
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int RecipePhaseTypeId { get; set; }
    public RecipePhaseType? RecipePhaseType { get; set; }

    public string? Description { get; set; }
    
    // Time in minutes (can be decimal)
    public double DurationMinutes { get; set; }
    
    // Specific for Cooking
    public int? Temperature { get; set; }
    public string? OvenMode { get; set; } // Statico, Ventilato...

    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    public List<PhaseIngredient> PhaseIngredients { get; set; } = new();
}

public class PhaseIngredient
{
    public int Id { get; set; }
    public string IngredientName { get; set; } = string.Empty; // Maps by name to main ingredient list
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;

    public int RecipePhaseId { get; set; }
    public RecipePhase? RecipePhase { get; set; }
}
