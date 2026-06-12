using AIOutfitStylist.Domain.Enums;

namespace AIOutfitStylist.Application.DTOs;

public sealed record GenerateOutfitRequest(Guid? PhotoAnalysisId, Occasion Occasion, decimal Budget, string Weather, string StylePreference, string? ShoppingDepartment);

public sealed record ProductDto(
    Guid Id,
    string ProductName,
    string Brand,
    decimal Price,
    decimal Rating,
    string PurchaseLink,
    string ProductImage,
    string Retailer,
    string Category);

public sealed record OutfitItemDto(
    Guid Id,
    string Category,
    string Name,
    string Color,
    decimal EstimatedPrice,
    string Notes,
    ProductDto? Product);

public sealed record OutfitDto(
    Guid Id,
    string Name,
    Occasion Occasion,
    decimal Budget,
    string Weather,
    string StylePreference,
    decimal EstimatedCost,
    string StylingExplanation,
    string? GeneratedImageUrl,
    IReadOnlyList<OutfitItemDto> Items,
    bool IsSaved);

public sealed record SaveOutfitRequest(Guid OutfitId, string? Notes);
