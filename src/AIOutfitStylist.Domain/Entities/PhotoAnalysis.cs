using AIOutfitStylist.Domain.Common;

namespace AIOutfitStylist.Domain.Entities;

public sealed class PhotoAnalysis : BaseEntity
{
    public Guid UserPhotoId { get; set; }
    public UserPhoto UserPhoto { get; set; } = default!;
    public string BodyType { get; set; } = string.Empty;
    public string SkinTone { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public string RecommendedColorsJson { get; set; } = "[]";
    public string RecommendationsJson { get; set; } = "[]";
    public string RecommendedCategoriesJson { get; set; } = "[]";
    public string RawAiResponseJson { get; set; } = "{}";
}
