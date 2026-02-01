namespace Ricettario.API.DTOs;

public class CreateRecipeDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public double PrepTimeMinutes { get; set; }
    public double CookTimeMinutes { get; set; }
    public string? Difficulty { get; set; }
    public string? ImageUrl { get; set; }
    
    public double TotalFlourWeight { get; set; }
    public int ServingPieces { get; set; }
    public double? PieceWeight { get; set; }
    
    // Category & Tags
    public int? CategoryId { get; set; }
    public List<int> TagIds { get; set; } = new();

    public List<IngredientDto> Ingredients { get; set; } = new();
    public List<RecipePhaseDto> Phases { get; set; } = new();
    public List<string> VideoUrls { get; set; } = new();
}

public class IngredientDto
{
    public string Name { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class RecipePhaseDto 
{
    public string Title { get; set; } = string.Empty;
    public int RecipePhaseTypeId { get; set; } // Renamed/Typed correctly
    public string? Description { get; set; }
    public double DurationMinutes { get; set; }
    public int? Temperature { get; set; }
    public string? OvenMode { get; set; }
    
    public List<PhaseIngredientDto> PhaseIngredients { get; set; } = new();
}

public class PhaseIngredientDto
{
    public string IngredientName { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}
