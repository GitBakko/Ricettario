namespace Ricettario.API.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Icon { get; set; } = "fa-solid fa-folder"; // Font Awesome icon class
    public string Color { get; set; } = "#6c757d"; // Hex color
    public int SortOrder { get; set; } = 0;
    public bool IsSystemDefault { get; set; } = false;
    
    // Navigation
    public List<Recipe> Recipes { get; set; } = new();
}
