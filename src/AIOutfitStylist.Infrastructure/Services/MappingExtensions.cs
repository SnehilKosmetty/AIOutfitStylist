using System.Text.Json;
using AIOutfitStylist.Application.DTOs;
using AIOutfitStylist.Domain.Entities;

namespace AIOutfitStylist.Infrastructure.Services;

internal static class MappingExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static UserProfileDto ToProfileDto(this User user) =>
        new(user.Id, user.FirstName, user.LastName, user.Email, user.Gender, user.Age, user.Height, user.Weight, user.ClothingSize, user.PreferredStyle, user.BudgetMin, user.BudgetMax);

    public static PhotoAnalysisDto ToDto(this PhotoAnalysis analysis) =>
        new(
            analysis.Id,
            analysis.UserPhotoId,
            analysis.BodyType,
            analysis.SkinTone,
            analysis.Style,
            ReadList(analysis.RecommendedColorsJson),
            ReadList(analysis.RecommendationsJson),
            ReadList(analysis.RecommendedCategoriesJson));

    public static ProductDto ToDto(this Product product) =>
        new(product.Id, product.ProductName, product.Brand, product.Price, product.Rating, product.PurchaseLink, product.ProductImage, product.Retailer, product.Category);

    public static OutfitDto ToDto(this Outfit outfit, bool isSaved) =>
        new(
            outfit.Id,
            outfit.Name,
            outfit.Occasion,
            outfit.Budget,
            outfit.Weather,
            outfit.StylePreference,
            outfit.EstimatedCost,
            outfit.StylingExplanation,
            outfit.GeneratedImageUrl,
            outfit.Items.Select(x => new OutfitItemDto(x.Id, x.Category, x.Name, x.Color, x.EstimatedPrice, x.Notes, x.Product?.ToDto())).ToList(),
            isSaved);

    public static string ToJson(this IReadOnlyList<string> values) => JsonSerializer.Serialize(values, JsonOptions);

    private static IReadOnlyList<string> ReadList(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<IReadOnlyList<string>>(json, JsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
