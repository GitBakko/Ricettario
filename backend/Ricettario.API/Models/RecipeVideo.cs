namespace Ricettario.API.Models;

public class RecipeVideo
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    
    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
}
