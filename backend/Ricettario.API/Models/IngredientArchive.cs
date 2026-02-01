using System.Text.Json;

namespace Ricettario.API.Models;

/// <summary>
/// Represents a reusable ingredient in the user's ingredient archive.
/// Contains custom properties (like W, Proteins for flour) and can have conversion ratios to other ingredients.
/// </summary>
public class IngredientArchive
{
    public int Id { get; set; }
    
    /// <summary>
    /// Display name (e.g., "Farina Manitoba", "Lievito Madre Solido")
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional description or notes
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Default unit of measurement (g, ml, unit, etc.)
    /// </summary>
    public string DefaultUnit { get; set; } = "g";
    
    /// <summary>
    /// Category for grouping (Farina, Lievito, Liquido, Grasso, Dolcificante, Altro)
    /// </summary>
    public IngredientCategory Category { get; set; } = IngredientCategory.Altro;
    
    /// <summary>
    /// FontAwesome icon class (e.g., "fa-solid fa-wheat-awn")
    /// </summary>
    public string? Icon { get; set; }
    
    /// <summary>
    /// Hex color for UI display
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// JSON storage for user-defined custom properties.
    /// Format: [{"Label": "W", "Value": "380"}, {"Label": "Proteine", "Value": "14%"}]
    /// </summary>
    public string? CustomPropertiesJson { get; set; }
    
    /// <summary>
    /// System defaults cannot be deleted by users
    /// </summary>
    public bool IsSystemDefault { get; set; } = false;
    
    /// <summary>
    /// Display order in lists
    /// </summary>
    public int SortOrder { get; set; } = 0;
    
    /// <summary>
    /// Conversions where this ingredient is the source
    /// </summary>
    public ICollection<IngredientConversion> ConversionsFrom { get; set; } = new List<IngredientConversion>();
    
    /// <summary>
    /// Conversions where this ingredient is the target
    /// </summary>
    public ICollection<IngredientConversion> ConversionsTo { get; set; } = new List<IngredientConversion>();
    
    // Helper methods for CustomProperties
    public List<IngredientCustomProperty> GetCustomProperties()
    {
        if (string.IsNullOrEmpty(CustomPropertiesJson))
            return new List<IngredientCustomProperty>();
        
        try
        {
            return JsonSerializer.Deserialize<List<IngredientCustomProperty>>(CustomPropertiesJson) 
                   ?? new List<IngredientCustomProperty>();
        }
        catch
        {
            return new List<IngredientCustomProperty>();
        }
    }
    
    public void SetCustomProperties(List<IngredientCustomProperty> properties)
    {
        CustomPropertiesJson = JsonSerializer.Serialize(properties);
    }
}

/// <summary>
/// Categories for ingredient grouping
/// </summary>
public enum IngredientCategory
{
    Farina = 0,
    Lievito = 1,
    Liquido = 2,
    Grasso = 3,
    Dolcificante = 4,
    Sale = 5,
    Uova = 6,
    Latticini = 7,
    Altro = 99
}

/// <summary>
/// User-defined custom property for an ingredient
/// </summary>
public class IngredientCustomProperty
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
