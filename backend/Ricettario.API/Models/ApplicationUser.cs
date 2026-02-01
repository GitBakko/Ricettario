using Microsoft.AspNetCore.Identity;

namespace Ricettario.API.Models;

public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User's display name shown in the app
    /// </summary>
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// Short biography or description
    /// </summary>
    public string? Bio { get; set; }
    
    /// <summary>
    /// URL to user's avatar image
    /// </summary>
    public string? AvatarUrl { get; set; }
    
    /// <summary>
    /// URL to user's profile banner image
    /// </summary>
    public string? BannerUrl { get; set; }
    
    /// <summary>
    /// User's location (city, country)
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// User's website or blog URL
    /// </summary>
    public string? Website { get; set; }
    
    /// <summary>
    /// Account creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last profile update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
