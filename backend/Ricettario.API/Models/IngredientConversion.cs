namespace Ricettario.API.Models;

/// <summary>
/// Defines a conversion ratio between two ingredients.
/// Bidirectional: if A→B has ratio 0.5, B→A automatically has ratio 2.0
/// </summary>
public class IngredientConversion
{
    public int Id { get; set; }
    
    /// <summary>
    /// Source ingredient ID
    /// </summary>
    public int FromIngredientId { get; set; }
    public IngredientArchive? FromIngredient { get; set; }
    
    /// <summary>
    /// Target ingredient ID
    /// </summary>
    public int ToIngredientId { get; set; }
    public IngredientArchive? ToIngredient { get; set; }
    
    /// <summary>
    /// Conversion ratio: ToQuantity = FromQuantity * ConversionRatio
    /// Example: 10g fresh yeast → 3g dry yeast = ratio 0.3
    /// </summary>
    public double ConversionRatio { get; set; } = 1.0;
    
    /// <summary>
    /// Optional notes about the conversion (e.g., "Aggiungere 50% di acqua in più")
    /// </summary>
    public string? Notes { get; set; }
}
