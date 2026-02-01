namespace Ricettario.API.DTOs;

/// <summary>
/// DTO for reading user profile data
/// </summary>
public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Stats
    public int RecipeCount { get; set; }
}

/// <summary>
/// DTO for updating user profile
/// </summary>
public class UpdateProfileDto
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
}

/// <summary>
/// DTO for updating user avatar
/// </summary>
public class UpdateAvatarDto
{
    public string AvatarUrl { get; set; } = string.Empty;
}
