using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ricettario.API.Data;
using Ricettario.API.DTOs;
using Ricettario.API.Models;
using System.Security.Claims;

namespace Ricettario.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    /// <summary>
    /// Get current user's profile
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found");

        var recipeCount = await _context.Recipes.CountAsync(r => r.UserId == userId);

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            BannerUrl = user.BannerUrl,
            Location = user.Location,
            Website = user.Website,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            RecipeCount = recipeCount
        });
    }

    /// <summary>
    /// Update current user's profile
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found");

        // Update fields
        user.DisplayName = dto.DisplayName?.Trim();
        user.Bio = dto.Bio?.Trim();
        user.Location = dto.Location?.Trim();
        user.Website = dto.Website?.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var recipeCount = await _context.Recipes.CountAsync(r => r.UserId == userId);

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            BannerUrl = user.BannerUrl,
            Location = user.Location,
            Website = user.Website,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            RecipeCount = recipeCount
        });
    }

    /// <summary>
    /// Update user's avatar
    /// </summary>
    [HttpPut("avatar")]
    public async Task<ActionResult<UserProfileDto>> UpdateAvatar([FromBody] UpdateAvatarDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found");

        user.AvatarUrl = dto.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var recipeCount = await _context.Recipes.CountAsync(r => r.UserId == userId);

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            BannerUrl = user.BannerUrl,
            Location = user.Location,
            Website = user.Website,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            RecipeCount = recipeCount
        });
    }

    /// <summary>
    /// Upload avatar image file
    /// </summary>
    [HttpPost("avatar/upload")]
    public async Task<ActionResult<UserProfileDto>> UploadAvatar(IFormFile file)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest("Invalid file type. Only JPEG, PNG, GIF, and WebP are allowed.");

        // Validate file size (max 5MB)
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size exceeds 5MB limit");

        // Generate unique filename
        var extension = Path.GetExtension(file.FileName).ToLower();
        var fileName = $"avatar_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
        
        // Ensure directory exists
        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Update user's avatar URL
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found");

        // Delete old avatar if exists
        if (!string.IsNullOrEmpty(user.AvatarUrl) && user.AvatarUrl.StartsWith("/uploads/"))
        {
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.AvatarUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
        }

        user.AvatarUrl = $"/uploads/avatars/{fileName}";
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var recipeCount = await _context.Recipes.CountAsync(r => r.UserId == userId);

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            BannerUrl = user.BannerUrl,
            Location = user.Location,
            Website = user.Website,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            RecipeCount = recipeCount
        });
    }

    /// <summary>
    /// Upload banner image file
    /// </summary>
    [HttpPost("banner/upload")]
    public async Task<ActionResult<UserProfileDto>> UploadBanner(IFormFile file)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest("Invalid file type. Only JPEG, PNG, GIF, and WebP are allowed.");

        // Validate file size (max 10MB for banners)
        if (file.Length > 10 * 1024 * 1024)
            return BadRequest("File size exceeds 10MB limit");

        // Generate unique filename
        var extension = Path.GetExtension(file.FileName).ToLower();
        var fileName = $"banner_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "banners");
        
        // Ensure directory exists
        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Update user's banner URL
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found");

        // Delete old banner if exists
        if (!string.IsNullOrEmpty(user.BannerUrl) && user.BannerUrl.StartsWith("/uploads/"))
        {
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.BannerUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
        }

        user.BannerUrl = $"/uploads/banners/{fileName}";
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var recipeCount = await _context.Recipes.CountAsync(r => r.UserId == userId);

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            BannerUrl = user.BannerUrl,
            Location = user.Location,
            Website = user.Website,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            RecipeCount = recipeCount
        });
    }
}
