namespace AIOutfitStylist.Application.DTOs;

public sealed record AdminDashboardDto(
    int TotalUsers,
    int TotalPhotos,
    int TotalAnalyses,
    int TotalOutfits,
    int SavedOutfits,
    int GeneratedToday,
    IReadOnlyList<AdminUserActivityDto> RecentUsers);

public sealed record AdminUserActivityDto(
    Guid UserId,
    string Name,
    string Email,
    string Gender,
    int? Age,
    decimal? Height,
    decimal? Weight,
    string? ClothingSize,
    string? PreferredStyle,
    decimal? BudgetMin,
    decimal? BudgetMax,
    DateTime CreatedAtUtc,
    int Photos,
    int Outfits,
    int SavedOutfits);
