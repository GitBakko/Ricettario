using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Ricettario.API.Models;
using System.Net.Http;
using SkiaSharp;

namespace Ricettario.API.Services;

public interface IPdfReportingService
{
    Task<byte[]> GenerateRecipePdf(Recipe recipe);
}

public class PdfReportingService : IPdfReportingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebHostEnvironment _env;

    public PdfReportingService(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
    {
        _httpClientFactory = httpClientFactory;
        _env = env;
    }

    public async Task<byte[]> GenerateRecipePdf(Recipe recipe)
    {
        // Pre-fetch and crop image to exact header dimensions
        byte[]? headerImageBytes = null;
        if (!string.IsNullOrEmpty(recipe.ImageUrl)) 
        {
            var rawImageBytes = await GetImageBytes(recipe.ImageUrl);
            if (rawImageBytes != null)
            {
                // Crop to 595x250 (A4 width x header height in points, ~1.4 pixel per pt for good quality)
                headerImageBytes = CropImageToFit(rawImageBytes, 834, 350);
            }
        }

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0); // Full bleed for header
                page.PageColor(Colors.White);
                
                // Fonts - using standard standard fonts for reliability, but styling them better
                var titleFont = "Arial"; 
                var bodyFont = "Calibri";

                page.Header().Element(ComposeHeader);

                page.Content().PaddingVertical(20).PaddingHorizontal(40).Column(column =>
                {
                    column.Spacing(20);

                    // Stats Grid
                    column.Item().Element(ComposeStats);
                    
                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten3);

                    // Ingredients Section (Sequential to avoid page break issues)
                    column.Item().Element(ComposeIngredients);
                    
                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten3);

                    // Phases Section
                    column.Item().Element(ComposePhases);
                });

                page.Footer()
                    .Padding(20)
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Ricettario AI - ").FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();

        void ComposeHeader(IContainer container)
        {
            container.Height(250).Layers(layers =>
            {
                // Primary Layer: Background (Image or Fallback Color)
                layers.PrimaryLayer().Element(bg => 
                {
                    if (headerImageBytes != null)
                    {
                        Console.WriteLine($"PDF: Rendering cropped header image, bytes={headerImageBytes.Length}");
                        bg.Image(headerImageBytes).FitArea();
                    }
                    else
                    {
                        Console.WriteLine("PDF: No image, using fallback background");
                        bg.Background(Colors.Grey.Darken4);
                    }
                });
                
                // Overlay Layer: Darken image for readability
                if (headerImageBytes != null)
                {
                    layers.Layer().Background("#AA000000"); // Semi-transparent black
                }

                // Content Layer
                layers.Layer().Padding(40).Column(column => 
                {
                    column.Item().Text("RICETTA").FontColor(Colors.Grey.Lighten1).FontSize(10).LetterSpacing(0.2f);
                    column.Item().Text(recipe.Title).FontColor(Colors.White).FontSize(32).Bold().FontFamily("Arial");
                    
                    if (!string.IsNullOrWhiteSpace(recipe.Description))
                    {
                        column.Item().PaddingTop(10).Text(recipe.Description)
                            .FontColor(Colors.Grey.Lighten3).FontSize(12).Italic();
                    }
                });
            });
        }

        void ComposeStats(IContainer container)
        {
            container.Row(row => 
            {
                row.RelativeItem().Column(c => {
                    c.Item().Text("TEMPO TOTALE").FontSize(9).FontColor(Colors.Grey.Medium).Bold();
                    c.Item().Text(FormatTime(GetTotalTime(recipe))).FontSize(14).SemiBold().FontColor(Colors.Grey.Darken3);
                });
                
                row.RelativeItem().Column(c => {
                    c.Item().Text("DIFFICOLTÀ").FontSize(9).FontColor(Colors.Grey.Medium).Bold();
                    c.Item().Text(recipe.Difficulty ?? "-").FontSize(14).SemiBold().FontColor(GetDifficultyColor(recipe.Difficulty));
                });

                row.RelativeItem().Column(c => {
                    c.Item().Text("RESA").FontSize(9).FontColor(Colors.Grey.Medium).Bold();
                    c.Item().Text($"{recipe.ServingPieces} pz x {recipe.PieceWeight:F0}g").FontSize(14).SemiBold().FontColor(Colors.Green.Darken2);
                });
            });
        }

        void ComposeIngredients(IContainer container)
        {
            container.Column(col => 
            {
                col.Item().PaddingBottom(10).Text("Ingredienti").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                
                // Ingredients Split Table
                col.Item().Table(table => 
                {
                    table.ColumnsDefinition(columns => 
                    {
                        columns.RelativeColumn(3); 
                        columns.RelativeColumn(1);
                        columns.ConstantColumn(20); 
                        columns.RelativeColumn(3); 
                        columns.RelativeColumn(1);
                    });

                    var count = recipe.Ingredients.Count;
                    var half = (int)Math.Ceiling(count / 2.0);
                    
                    for (int i = 0; i < half; i++)
                    {
                        var left = recipe.Ingredients[i];
                        var right = (i + half < count) ? recipe.Ingredients[i + half] : null;

                        AddIngredientCell(table, left);
                        table.Cell(); // Spacer
                        if (right != null) AddIngredientCell(table, right);
                        else { table.Cell(); table.Cell(); }
                    }
                });
            });
        }

        void AddIngredientCell(TableDescriptor table, dynamic ing)
        {
             string name = ing.Name;
             bool isPhaseRef = name.StartsWith("PHASE:");
             string displayName = isPhaseRef ? "Referenza Fase" : name;

             table.Cell().PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Text(displayName).SemiBold().FontSize(10).FontColor(Colors.Grey.Darken3);
             table.Cell().PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).AlignRight().Text(t => {
                 t.Span($"{ing.Quantity:F1}").Bold();
                 t.Span($" {ing.Unit}").FontSize(9).FontColor(Colors.Grey.Darken1);
             });
        }

        void ComposePhases(IContainer container)
        {
            container.Column(col => 
            {
                col.Item().PaddingBottom(15).Text("Procedimento").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                
                if (recipe.Phases != null && recipe.Phases.Any())
                {
                    int phaseIndex = 1;
                    foreach (var phase in recipe.Phases)
                    {
                        var phaseColor = phase.RecipePhaseType?.Color ?? "#6c757d";
                        
                        // Phase Item Wrapper - ShowEntire keeps phase on same page if it fits
                        col.Item().ShowEntire().PaddingBottom(20).Row(row => 
                        {
                            // Left Visual Timeline
                            row.AutoItem().PaddingRight(15).Column(c => {
                                // Dot with Index Number
                                c.Item().Width(24).Height(24).Border(2).BorderColor(phaseColor)
                                 .Background(Colors.White).AlignCenter().AlignMiddle()
                                 .Text($"{phaseIndex++}").FontSize(10).Bold().FontColor(phaseColor);
                                
                                // Line connecting to next (but we just draw it down)
                                c.Item().AlignCenter().Width(2).Height(50).Background(Colors.Grey.Lighten2);
                            });

                            // Content
                            row.RelativeItem().Column(c => 
                            {
                                c.Item().Text(t => {
                                    t.Span(phase.Title.ToUpperInvariant()).FontSize(11).Bold().FontColor(phaseColor);
                                });
                                
                                c.Item().PaddingBottom(5).Row(meta => {
                                     meta.AutoItem().Text(FormatTime(phase.DurationMinutes)).FontSize(10).Bold().FontColor(Colors.Grey.Darken3);
                                     if (phase.Temperature > 0)
                                     {
                                         meta.AutoItem().PaddingLeft(10).Text("|").FontColor(Colors.Grey.Lighten1);
                                         meta.AutoItem().PaddingLeft(10).Text($"{phase.Temperature}°C").FontSize(10).FontColor(Colors.Red.Medium);
                                     }
                                     if (!string.IsNullOrWhiteSpace(phase.OvenMode))
                                     {
                                         meta.AutoItem().PaddingLeft(5).Text($"({phase.OvenMode})").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                                     }
                                });
                                
                                if (!string.IsNullOrWhiteSpace(phase.Description))
                                    c.Item().AlignLeft().Text(phase.Description).FontSize(11).FontColor(Colors.Grey.Darken4).LineHeight(1.4f);
                                
                                // Filter out phase references (PHASE:X) from ingredients
                                var realIngredients = phase.PhaseIngredients?.Where(pi => !pi.IngredientName.StartsWith("PHASE:")).ToList();
                                if (realIngredients != null && realIngredients.Any())
                                {
                                    c.Item().PaddingTop(8).Background(Colors.Grey.Lighten5).Padding(8).Column(piCol => {
                                        piCol.Item().PaddingBottom(4).Text("Ingredienti in questa fase:").FontSize(8).Bold().FontColor(Colors.Grey.Medium);
                                        
                                        // Use table for better alignment like main list
                                        piCol.Item().Table(table => {
                                            table.ColumnsDefinition(cd => {
                                                cd.RelativeColumn(3); // Name
                                                cd.RelativeColumn(1); // Qty
                                            });
                                            foreach (var pi in realIngredients)
                                            {
                                                 table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(2).Text(pi.IngredientName).FontSize(9).FontColor(Colors.Grey.Darken3);
                                                 table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(2).AlignRight().Text($"{pi.Quantity:F1} {pi.Unit}").FontSize(9).FontColor(Colors.Grey.Darken1);
                                            }
                                        });
                                    });
                                }
                            });
                        });
                    }
                }
                else
                {
                    col.Item().Text(recipe.Instructions).FontSize(10);
                }
            });
        }
    }

    private async Task<byte[]?> GetImageBytes(string url)
    {
        try {
            Console.WriteLine($"PDF GetImageBytes: Starting with URL={url}");
            
            // Priority: Local File System (faster & reliable for localhost)
            // If URL is like "http://localhost:5254/images/recipes/uuid.jpg"
            if (url.Contains("/images/recipes/"))
            {
               var fileName = url.Split('/').Last();
               // Ensure WebRootPath is safe, fallback if needed
               var webRoot = !string.IsNullOrEmpty(_env.WebRootPath) ? _env.WebRootPath : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
               var filePath = Path.Combine(webRoot, "images", "recipes", fileName);
               Console.WriteLine($"PDF GetImageBytes: Local path={filePath}, Exists={File.Exists(filePath)}");
               if (File.Exists(filePath))
               {
                   var bytes = await File.ReadAllBytesAsync(filePath);
                   Console.WriteLine($"PDF GetImageBytes: Loaded {bytes.Length} bytes from local file");
                   return bytes;
               }
            }

            // Fallback: HTTP Fetch (for external images)
            if (url.StartsWith("http"))
            {
                Console.WriteLine($"PDF GetImageBytes: Attempting HTTP fetch for {url}");
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                var bytes = await client.GetByteArrayAsync(url);
                Console.WriteLine($"PDF GetImageBytes: HTTP loaded {bytes.Length} bytes");
                return bytes;
            }
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"PDF Image error: {ex.Message} - Stack: {ex.StackTrace}");
        }
        Console.WriteLine($"PDF GetImageBytes: Returning null for {url}");
        return null;
    }

    private string FormatTime(double minutes)
    {
        if (minutes < 60) return $"{minutes} min";
        var h = (int)(minutes / 60);
        var m = (int)(minutes % 60);
        return m > 0 ? $"{h}h {m}m" : $"{h}h";
    }

    private string GetDifficultyColor(string? diff)
    {
        return diff switch {
            "Easy" => Colors.Green.Medium,
            "Medium" => Colors.Orange.Medium,
            "Hard" => Colors.Red.Medium,
            _ => Colors.Grey.Medium
        };
    }


    private double GetTotalTime(Recipe r)
    {
        return r.Phases?.Sum(p => p.DurationMinutes) ?? (r.PrepTimeMinutes + r.CookTimeMinutes);
    }

    /// <summary>
    /// Crops the source image to fit the target dimensions using center crop
    /// </summary>
    private byte[]? CropImageToFit(byte[] sourceBytes, int targetWidth, int targetHeight)
    {
        try
        {
            using var sourceBitmap = SKBitmap.Decode(sourceBytes);
            if (sourceBitmap == null) 
            {
                Console.WriteLine("PDF CropImageToFit: Failed to decode source image - returning null");
                return null; // Return null instead of invalid bytes
            }

            float targetAspect = (float)targetWidth / targetHeight;
            float sourceAspect = (float)sourceBitmap.Width / sourceBitmap.Height;

            int cropWidth, cropHeight, cropX, cropY;

            if (sourceAspect > targetAspect)
            {
                // Source is wider - crop sides
                cropHeight = sourceBitmap.Height;
                cropWidth = (int)(cropHeight * targetAspect);
                cropX = (sourceBitmap.Width - cropWidth) / 2;
                cropY = 0;
            }
            else
            {
                // Source is taller - crop top/bottom
                cropWidth = sourceBitmap.Width;
                cropHeight = (int)(cropWidth / targetAspect);
                cropX = 0;
                cropY = (sourceBitmap.Height - cropHeight) / 2;
            }

            var cropRect = new SKRectI(cropX, cropY, cropX + cropWidth, cropY + cropHeight);
            
            using var croppedBitmap = new SKBitmap(targetWidth, targetHeight);
            using var canvas = new SKCanvas(croppedBitmap);
            
            var destRect = new SKRect(0, 0, targetWidth, targetHeight);
            var sourceRect = new SKRect(cropRect.Left, cropRect.Top, cropRect.Right, cropRect.Bottom);
            
            canvas.DrawBitmap(sourceBitmap, sourceRect, destRect, new SKPaint { FilterQuality = SKFilterQuality.High });

            using var image = SKImage.FromBitmap(croppedBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);
            
            Console.WriteLine($"PDF CropImageToFit: Cropped from {sourceBitmap.Width}x{sourceBitmap.Height} to {targetWidth}x{targetHeight}");
            return data.ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PDF CropImageToFit error: {ex.Message}");
            return null; // Return null on error - will use fallback background
        }
    }
}
