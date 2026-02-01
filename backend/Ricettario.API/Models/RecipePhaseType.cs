namespace Ricettario.API.Models;

public class RecipePhaseType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Styling
    public string? Icon { get; set; } = "fa-solid fa-circle";
    public string? Color { get; set; } = "#6c757d"; // Hex or Bootstrap class

    public bool IsActiveWork { get; set; }
    public bool IsSystemDefault { get; set; }
}
