using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ricettario.API.Models;

namespace Ricettario.API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<RecipePhase> RecipePhases { get; set; }
    public DbSet<RecipePhaseType> RecipePhaseTypes { get; set; }
    public DbSet<RecipeVideo> RecipeVideos { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<RecipeTag> RecipeTags { get; set; }
    public DbSet<IngredientArchive> IngredientArchives { get; set; }
    public DbSet<IngredientConversion> IngredientConversions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure relationships
        builder.Entity<Recipe>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Ingredient>()
            .HasOne(i => i.Recipe)
            .WithMany(r => r.Ingredients)
            .HasForeignKey(i => i.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<RecipePhase>()
            .HasOne(p => p.RecipePhaseType)
            .WithMany()
            .HasForeignKey(p => p.RecipePhaseTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Category - Recipe relationship
        builder.Entity<Recipe>()
            .HasOne(r => r.Category)
            .WithMany(c => c.Recipes)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Recipe-Tag Many-to-Many
        builder.Entity<RecipeTag>()
            .HasKey(rt => new { rt.RecipeId, rt.TagId });

        builder.Entity<RecipeTag>()
            .HasOne(rt => rt.Recipe)
            .WithMany(r => r.RecipeTags)
            .HasForeignKey(rt => rt.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RecipeTag>()
            .HasOne(rt => rt.Tag)
            .WithMany(t => t.RecipeTags)
            .HasForeignKey(rt => rt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seeding Phase Types with Font Awesome icons
        builder.Entity<RecipePhaseType>().HasData(
            new RecipePhaseType { Id = 1, Name = "Impasto", Icon = "fa-solid fa-compact-disc", Color = "#0d6efd", IsActiveWork = true, IsSystemDefault = true },
            new RecipePhaseType { Id = 2, Name = "Lievitazione", Icon = "fa-solid fa-hourglass-half", Color = "#ffc107", IsActiveWork = false, IsSystemDefault = true },
            new RecipePhaseType { Id = 3, Name = "Cottura", Icon = "fa-solid fa-fire", Color = "#dc3545", IsActiveWork = true, IsSystemDefault = true },
            new RecipePhaseType { Id = 4, Name = "Pre-Impasto", Icon = "fa-solid fa-basket-shopping", Color = "#6c757d", IsActiveWork = true, IsSystemDefault = true },
            new RecipePhaseType { Id = 5, Name = "Pieghe", Icon = "fa-solid fa-layer-group", Color = "#198754", IsActiveWork = true, IsSystemDefault = true },
            new RecipePhaseType { Id = 6, Name = "Formatura", Icon = "fa-solid fa-box", Color = "#0dcaf0", IsActiveWork = true, IsSystemDefault = true },
            new RecipePhaseType { Id = 7, Name = "Appretto", Icon = "fa-solid fa-rotate-right", Color = "#fd7e14", IsActiveWork = false, IsSystemDefault = true },
            new RecipePhaseType { Id = 8, Name = "Autoli", Icon = "fa-solid fa-droplet", Color = "#20c997", IsActiveWork = false, IsSystemDefault = true }
        );

        // Seeding Categories
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Pizza", Description = "Pizze classiche, gourmet e regionali", Icon = "fa-solid fa-pizza-slice", Color = "#dc3545", SortOrder = 1, IsSystemDefault = true },
            new Category { Id = 2, Name = "Pane", Description = "Pane artigianale, filoni, pagnotte", Icon = "fa-solid fa-bread-slice", Color = "#8B4513", SortOrder = 2, IsSystemDefault = true },
            new Category { Id = 3, Name = "Focaccia", Description = "Focacce liguri, pugliesi e varianti", Icon = "fa-solid fa-border-all", Color = "#f0ad4e", SortOrder = 3, IsSystemDefault = true },
            new Category { Id = 4, Name = "Pasticceria", Description = "Dolci da forno, crostate, torte", Icon = "fa-solid fa-cake-candles", Color = "#e91e8c", SortOrder = 4, IsSystemDefault = true },
            new Category { Id = 5, Name = "Grandi Lievitati", Description = "Panettone, pandoro, colomba", Icon = "fa-solid fa-star", Color = "#ffc107", SortOrder = 5, IsSystemDefault = true },
            new Category { Id = 6, Name = "Brioche", Description = "Brioches dolci e salate, croissant", Icon = "fa-solid fa-moon", Color = "#fd7e14", SortOrder = 6, IsSystemDefault = true },
            new Category { Id = 7, Name = "Biscotti", Description = "Biscotti, frollini, cantucci", Icon = "fa-solid fa-cookie", Color = "#795548", SortOrder = 7, IsSystemDefault = true },
            new Category { Id = 8, Name = "Torte Salate", Description = "Quiche, torte rustiche, sfoglie", Icon = "fa-solid fa-hexagon", Color = "#28a745", SortOrder = 8, IsSystemDefault = true },
            new Category { Id = 9, Name = "Pasta Fresca", Description = "Pasta all'uovo, ravioli, gnocchi", Icon = "fa-solid fa-layer-group", Color = "#17a2b8", SortOrder = 9, IsSystemDefault = true },
            new Category { Id = 10, Name = "Viennoiserie", Description = "Pain au chocolat, danish, sfoglia", Icon = "fa-solid fa-diamond", Color = "#c7a17a", SortOrder = 10, IsSystemDefault = true },
            new Category { Id = 11, Name = "Altro", Description = "Altre preparazioni da forno", Icon = "fa-solid fa-ellipsis", Color = "#6c757d", SortOrder = 99, IsSystemDefault = true }
        );

        // =========================================================================
        // Ingredient Archive Configuration
        // =========================================================================
        
        builder.Entity<IngredientArchive>(entity =>
        {
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Category);
            
            entity.Property(e => e.Category)
                .HasConversion<int>();
        });

        // IngredientConversion - self-referencing many-to-many
        builder.Entity<IngredientConversion>(entity =>
        {
            entity.HasOne(c => c.FromIngredient)
                .WithMany(i => i.ConversionsFrom)
                .HasForeignKey(c => c.FromIngredientId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(c => c.ToIngredient)
                .WithMany(i => i.ConversionsTo)
                .HasForeignKey(c => c.ToIngredientId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade loop
            
            // Unique constraint: only one conversion per pair
            entity.HasIndex(c => new { c.FromIngredientId, c.ToIngredientId }).IsUnique();
        });

        // Ingredient → IngredientArchive (optional link for backward compatibility)
        builder.Entity<Ingredient>()
            .HasOne(i => i.ArchiveIngredient)
            .WithMany()
            .HasForeignKey(i => i.ArchiveIngredientId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed common yeast types
        builder.Entity<IngredientArchive>().HasData(
            new IngredientArchive 
            { 
                Id = 1, 
                Name = "Lievito di Birra Fresco", 
                Category = IngredientCategory.Lievito,
                Icon = "fa-solid fa-cubes",
                Color = "#FFC107",
                IsSystemDefault = true,
                SortOrder = 1
            },
            new IngredientArchive 
            { 
                Id = 2, 
                Name = "Lievito di Birra Secco", 
                Category = IngredientCategory.Lievito,
                Icon = "fa-solid fa-cubes-stacked",
                Color = "#FF9800",
                IsSystemDefault = true,
                SortOrder = 2
            },
            new IngredientArchive 
            { 
                Id = 3, 
                Name = "Lievito Madre Solido", 
                Category = IngredientCategory.Lievito,
                Icon = "fa-solid fa-bread-slice",
                Color = "#8D6E63",
                IsSystemDefault = true,
                SortOrder = 3
            },
            new IngredientArchive 
            { 
                Id = 4, 
                Name = "Licoli (Lievito Madre Liquido)", 
                Category = IngredientCategory.Lievito,
                Icon = "fa-solid fa-droplet",
                Color = "#A1887F",
                IsSystemDefault = true,
                SortOrder = 4
            }
        );

        // Seed yeast conversion ratios
        builder.Entity<IngredientConversion>().HasData(
            // Fresh yeast → Dry yeast (ratio 0.33 - 10g fresh = 3.3g dry)
            new IngredientConversion { Id = 1, FromIngredientId = 1, ToIngredientId = 2, ConversionRatio = 0.33, Notes = "Il lievito secco è più concentrato" },
            // Fresh yeast → Lievito Madre Solido (ratio 3.0 - 10g fresh = 30g LM)
            new IngredientConversion { Id = 2, FromIngredientId = 1, ToIngredientId = 3, ConversionRatio = 3.0, Notes = "Aumentare tempo lievitazione" },
            // Fresh yeast → Licoli (ratio 3.0 - 10g fresh = 30g Licoli)
            new IngredientConversion { Id = 3, FromIngredientId = 1, ToIngredientId = 4, ConversionRatio = 3.0, Notes = "Ridurre liquidi del 15%" },
            // Dry yeast → Fresh yeast (explicit inverse)
            new IngredientConversion { Id = 4, FromIngredientId = 2, ToIngredientId = 1, ConversionRatio = 3.0 },
            // LM Solido → Licoli (ratio 1.0, same weight)
            new IngredientConversion { Id = 5, FromIngredientId = 3, ToIngredientId = 4, ConversionRatio = 1.0, Notes = "Aggiungere 50% acqua per compensare idratazione" }
        );
    }
}
