namespace AIOutfitStylist.Application.DTOs;

public sealed record PhotoUploadResponse(Guid PhotoId, string FileName, string BlobUrl, long SizeBytes, DateTime CreatedAtUtc);

public sealed record PhotoAnalysisDto(
    Guid Id,
    Guid PhotoId,
    string BodyType,
    string SkinTone,
    string Style,
    IReadOnlyList<string> RecommendedColors,
    IReadOnlyList<string> Recommendations,
    IReadOnlyList<string> RecommendedCategories);
