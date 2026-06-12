using AIOutfitStylist.Application.DTOs;
using AIOutfitStylist.Application.Interfaces;
using AIOutfitStylist.Domain.Entities;
using AIOutfitStylist.Domain.Enums;
using AIOutfitStylist.Infrastructure.Options;
using AIOutfitStylist.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AIOutfitStylist.Infrastructure.Services;

public sealed class AiStylistService(ApplicationDbContext dbContext, IProductSearchService productSearchService, HttpClient httpClient, IOptions<OpenAiOptions> options, IWebHostEnvironment environment) : IAiStylistService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly OpenAiOptions _options = options.Value;

    public async Task<PhotoAnalysisDto> AnalyzePhotoAsync(Guid userId, Guid photoId, CancellationToken cancellationToken = default)
    {
        var photo = await dbContext.UserPhotos
            .Include(x => x.Analysis)
            .FirstOrDefaultAsync(x => x.Id == photoId && x.UserId == userId, cancellationToken)
            ?? throw new InvalidOperationException("Photo not found.");

        if (photo.Analysis is not null)
        {
            return photo.Analysis.ToDto();
        }

        var analysis = string.IsNullOrWhiteSpace(_options.ApiKey)
            ? CreateFallbackAnalysis(photo.Id)
            : await AnalyzeWithOpenAiAsync(photo, cancellationToken);

        await dbContext.PhotoAnalyses.AddAsync(analysis, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return analysis.ToDto();
    }

    private async Task<PhotoAnalysis> AnalyzeWithOpenAiAsync(UserPhoto photo, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        var imageUrl = await ResolveOpenAiImageUrlAsync(photo, cancellationToken);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            model = _options.VisionModel,
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content = "You are a fashion stylist. Return strict JSON with bodyType, skinTone, style, recommendedColors, recommendations, and recommendedCategories."
                },
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = "Analyze this outfit styling photo for clothing recommendations. Avoid sensitive identity inferences." },
                        new { type = "image_url", image_url = new { url = imageUrl } }
                    }
                }
            }
        }, JsonOptions), Encoding.UTF8, "application/json");

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "{}";
        using var payload = JsonDocument.Parse(content);
        var root = payload.RootElement;

        return new PhotoAnalysis
        {
            UserPhotoId = photo.Id,
            BodyType = root.TryGetProperty("bodyType", out var bodyType) ? bodyType.GetString() ?? "" : "",
            SkinTone = root.TryGetProperty("skinTone", out var skinTone) ? skinTone.GetString() ?? "" : "",
            Style = root.TryGetProperty("style", out var style) ? style.GetString() ?? "" : "",
            RecommendedColorsJson = root.TryGetProperty("recommendedColors", out var colors) ? colors.GetRawText() : "[]",
            RecommendationsJson = root.TryGetProperty("recommendations", out var recommendations) ? recommendations.GetRawText() : "[]",
            RecommendedCategoriesJson = root.TryGetProperty("recommendedCategories", out var categories) ? categories.GetRawText() : "[]",
            RawAiResponseJson = json
        };
    }

    private async Task<string> ResolveOpenAiImageUrlAsync(UserPhoto photo, CancellationToken cancellationToken)
    {
        if (photo.BlobUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return photo.BlobUrl;
        }

        var localPath = photo.BlobUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var filePath = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), localPath);
        if (!File.Exists(filePath))
        {
            throw new InvalidOperationException("Uploaded image file was not found on the API server.");
        }

        var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
        return $"data:{photo.ContentType};base64,{Convert.ToBase64String(bytes)}";
    }

    private static PhotoAnalysis CreateFallbackAnalysis(Guid photoId) =>
        new()
        {
            UserPhotoId = photoId,
            BodyType = "Balanced",
            SkinTone = "Neutral warm",
            Style = "Modern smart casual",
            RecommendedColorsJson = new[] { "navy", "cream", "olive", "soft white", "charcoal" }.ToJson(),
            RecommendationsJson = new[] { "structured layers", "mid-rise bottoms", "clean sneakers or loafers", "minimal accessories" }.ToJson(),
            RecommendedCategoriesJson = new[] { "Shirt/Top", "Bottom", "Shoes", "Accessories" }.ToJson(),
            RawAiResponseJson = "{\"source\":\"local-fallback\",\"model\":\"openai-vision-ready\"}"
        };

    public async Task<IReadOnlyList<OutfitDto>> GenerateOutfitsAsync(Guid userId, GenerateOutfitRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        var analysis = request.PhotoAnalysisId.HasValue
            ? await dbContext.PhotoAnalyses.AsNoTracking().Include(x => x.UserPhoto).FirstOrDefaultAsync(x => x.Id == request.PhotoAnalysisId, cancellationToken)
            : null;

        var colors = analysis?.ToDto().RecommendedColors ?? GetPreferenceColors(request.StylePreference, request.Occasion);
        var department = ResolveDepartment(request.ShoppingDepartment, user.Gender);
        var categories = new[] { "Shirt/Top", "Bottom", "Shoes", "Accessories" };
        var outfitNames = new[] { "Dinner Smart", "Relaxed Sharp", "Modern Night Out" };
        var allocations = new[] { 0.32m, 0.30m, 0.28m, 0.10m };
        var results = new List<OutfitDto>();

        for (var i = 0; i < 3; i++)
        {
            var outfit = new Outfit
            {
                UserId = userId,
                PhotoAnalysisId = analysis?.Id,
                Name = $"{outfitNames[i]} {request.Occasion}",
                Occasion = request.Occasion,
                Budget = request.Budget,
                Weather = request.Weather,
                StylePreference = request.StylePreference,
                StylingExplanation = $"A {department} {request.Occasion} outfit for {request.Weather} weather with {request.StylePreference} styling. The palette uses {string.Join(", ", colors.Skip(i).Concat(colors).Take(3))} so each option feels distinct while staying wearable."
            };

            for (var c = 0; c < categories.Length; c++)
            {
                var maxPrice = Math.Round(request.Budget * allocations[c], 2);
                var itemColor = colors[(i + c) % colors.Count];
                var products = await productSearchService.SearchAsync(categories[c], request.StylePreference, maxPrice, department, itemColor, cancellationToken);
                var affordableProducts = products.Where(x => x.Price <= maxPrice).OrderByDescending(x => x.Rating).ThenBy(x => x.Price).ToList();
                var productDto = affordableProducts.Count > 0
                    ? affordableProducts[(i + c) % affordableProducts.Count]
                    : products.OrderBy(x => x.Price).First();
                var product = new Product
                {
                    ProductName = productDto.ProductName,
                    Brand = productDto.Brand,
                    Price = productDto.Price,
                    Rating = productDto.Rating,
                    PurchaseLink = productDto.PurchaseLink,
                    ProductImage = productDto.ProductImage,
                    Retailer = productDto.Retailer,
                    Category = productDto.Category
                };

                outfit.Items.Add(new OutfitItem
                {
                    Category = categories[c],
                    Name = product.ProductName,
                    Color = itemColor,
                    EstimatedPrice = product.Price,
                    Notes = $"Chosen to support the {request.StylePreference} direction while staying inside the item budget.",
                    Product = product
                });
            }

            outfit.EstimatedCost = outfit.Items.Sum(x => x.EstimatedPrice);
            if (outfit.EstimatedCost > request.Budget)
            {
                var overage = outfit.EstimatedCost - request.Budget;
                outfit.Items.OrderByDescending(x => x.EstimatedPrice).First().EstimatedPrice -= overage;
                outfit.EstimatedCost = outfit.Items.Sum(x => x.EstimatedPrice);
            }

            if (analysis?.UserPhoto is not null && !string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                outfit.GeneratedImageUrl = await TryGenerateVirtualTryOnAsync(analysis.UserPhoto, outfit, department, cancellationToken);
            }
            else if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                outfit.GeneratedImageUrl = await TryGenerateAvatarOutfitAsync(outfit, department, cancellationToken);
            }

            await dbContext.Outfits.AddAsync(outfit, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            results.Add(outfit.ToDto(false));
        }

        return results;
    }

    private async Task<string?> TryGenerateVirtualTryOnAsync(UserPhoto photo, Outfit outfit, string department, CancellationToken cancellationToken)
    {
        try
        {
            var imagePath = ResolveLocalImagePath(photo);
            if (imagePath is null || !File.Exists(imagePath))
            {
                return null;
            }

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(_options.ImageModel), "model");
            form.Add(new StringContent("1024x1536"), "size");
            form.Add(new StringContent("low"), "quality");
            form.Add(new StringContent("high"), "input_fidelity");
            form.Add(new StringContent(BuildTryOnPrompt(outfit, department)), "prompt");

            await using var imageStream = File.OpenRead(imagePath);
            using var imageContent = new StreamContent(imageStream);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue(photo.ContentType);
            form.Add(imageContent, "image", Path.GetFileName(imagePath));

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/images/edits");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            request.Content = form;

            using var response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(json);
            var b64 = document.RootElement.GetProperty("data")[0].GetProperty("b64_json").GetString();
            if (string.IsNullOrWhiteSpace(b64))
            {
                return null;
            }

            var outputRoot = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "generated");
            Directory.CreateDirectory(outputRoot);
            var outputName = $"{Guid.NewGuid():N}.png";
            await File.WriteAllBytesAsync(Path.Combine(outputRoot, outputName), Convert.FromBase64String(b64), cancellationToken);
            return $"/generated/{outputName}";
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> TryGenerateAvatarOutfitAsync(Outfit outfit, string department, CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/images/generations");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                model = _options.ImageModel,
                size = "1024x1536",
                quality = "low",
                prompt = BuildAvatarPrompt(outfit, department)
            }, JsonOptions), Encoding.UTF8, "application/json");

            using var response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(json);
            var b64 = document.RootElement.GetProperty("data")[0].GetProperty("b64_json").GetString();
            if (string.IsNullOrWhiteSpace(b64))
            {
                return null;
            }

            var outputRoot = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "generated");
            Directory.CreateDirectory(outputRoot);
            var outputName = $"{Guid.NewGuid():N}.png";
            await File.WriteAllBytesAsync(Path.Combine(outputRoot, outputName), Convert.FromBase64String(b64), cancellationToken);
            return $"/generated/{outputName}";
        }
        catch
        {
            return null;
        }
    }

    private string? ResolveLocalImagePath(UserPhoto photo)
    {
        if (photo.BlobUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var localPath = photo.BlobUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), localPath);
    }

    private static string BuildTryOnPrompt(Outfit outfit, string department)
    {
        var items = string.Join(", ", outfit.Items.Select(item => $"{item.Color} {item.Category.ToLowerInvariant()}"));
        return $"Create a realistic virtual outfit preview using the uploaded photo only as a body-proportion and pose reference. Do not recreate the person's face and do not generate a random face. Use a clean faceless fashion avatar or mannequin with the approximate body shape and stance from the input. Dress the avatar in a tasteful {department} outfit for {outfit.Occasion}: {items}. Show the full outfit clearly, realistic fabric and fit, simple studio or mirror-photo background, no text, no logos, no extra people.";
    }

    private static string BuildAvatarPrompt(Outfit outfit, string department)
    {
        var items = string.Join(", ", outfit.Items.Select(item => $"{item.Color} {item.Category.ToLowerInvariant()}"));
        return $"Create a realistic full-body fashion preview on a clean faceless avatar/mannequin. Outfit type: {department} {outfit.Occasion}. Clothing: {items}. Style: {outfit.StylePreference}. Weather: {outfit.Weather}. Show the outfit clearly from head to toe, realistic fabric and fit, simple studio background, no face details, no text, no logos, no extra people.";
    }

    private static string ResolveDepartment(string? requestedDepartment, Gender profileGender)
    {
        var normalized = requestedDepartment?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "mens" => "men's",
            "womens" => "women's",
            "unisex" => "unisex",
            _ => ResolveDepartment(profileGender)
        };
    }

    private static string ResolveDepartment(Gender gender) => gender switch
    {
        Gender.Male => "men's",
        Gender.Female => "women's",
        _ => "unisex"
    };

    private static IReadOnlyList<string> GetPreferenceColors(string stylePreference, Occasion occasion)
    {
        var style = stylePreference.ToLowerInvariant();
        if (occasion is Occasion.DateNight or Occasion.Party or Occasion.Wedding)
        {
            return ["black", "cream", "charcoal", "white", "brown"];
        }

        if (style.Contains("casual"))
        {
            return ["navy", "white", "olive", "light blue", "brown"];
        }

        if (style.Contains("formal") || style.Contains("office") || style.Contains("interview"))
        {
            return ["charcoal", "white", "navy", "black", "brown"];
        }

        return ["navy", "white", "olive", "charcoal", "cream"];
    }
}
