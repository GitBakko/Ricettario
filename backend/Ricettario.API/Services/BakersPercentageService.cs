using Ricettario.API.Models;

namespace Ricettario.API.Services;

public interface IBakersPercentageService
{
    void CalculatePercentages(Recipe recipe);
    Dictionary<string, double> ResizeByFlour(Recipe recipe, double newFlourWeight);
    Dictionary<string, double> ResizeByPieces(Recipe recipe, int numberOfPieces, double pieceWeight);
}

public class BakersPercentageService : IBakersPercentageService
{
    public void CalculatePercentages(Recipe recipe)
    {
        // 1. Identify Total Flour. 
        // Logic: Usually ingredients named 'Floor', 'Farina', 'Semola' etc. 
        // For MVP, we assume the user marks 'TotalFlourWeight' explicitly on the Recipe or we sum specific ingredients if we add tags later.
        // The instructions say: "user inputs ingredients -> app calculates percentages vs total flour".
        // We will assume that the 'TotalFlourWeight' property on Recipe is the reference.
        
        if (recipe.TotalFlourWeight <= 0) return;

        foreach (var ingredient in recipe.Ingredients)
        {
            if (ingredient.Quantity > 0)
            {
                ingredient.BakersPercentage = (ingredient.Quantity / recipe.TotalFlourWeight) * 100;
            }
        }
    }

    public Dictionary<string, double> ResizeByFlour(Recipe recipe, double newFlourWeight)
    {
        var result = new Dictionary<string, double>();
        foreach (var ing in recipe.Ingredients)
        {
            // New Weight = (Percentage * NewFlour) / 100
            double newWeight = (ing.BakersPercentage * newFlourWeight) / 100;
            result.Add(ing.Name, Math.Round(newWeight, 2));
        }
        return result;
    }

    public Dictionary<string, double> ResizeByPieces(Recipe recipe, int numberOfPieces, double pieceWeight)
    {
        // 1. Calculate Target Total Dough Weight
        double targetTotalWeight = numberOfPieces * pieceWeight;

        // 2. Calculate Current Total Percentage Sum (Flour is 100%)
        // Verify loop: Sum of (Percentage) for all ingredients.
        // Note: Flour itself should be in the ingredient list with 100% usually, or handled implicitly.
        // If the ingredient list contains the flour, its % is 100.
        
        double totalPercentage = recipe.Ingredients.Sum(i => i.BakersPercentage);
        
        // 3. Calculate New Flour Weight
        // TotalWeight = FlourWeight * (TotalPercentage / 100)
        // => FlourWeight = TotalWeight / (TotalPercentage / 100)
        
        if (totalPercentage == 0) return new Dictionary<string, double>();

        double newFlourWeight = targetTotalWeight / (totalPercentage / 100);

        return ResizeByFlour(recipe, newFlourWeight);
    }
}
