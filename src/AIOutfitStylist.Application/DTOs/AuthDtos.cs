using AIOutfitStylist.Domain.Enums;

namespace AIOutfitStylist.Application.DTOs;

public sealed record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string OtpCode,
    Gender Gender,
    int? Age,
    decimal? Height,
    decimal? Weight,
    string? ClothingSize,
    string? PreferredStyle,
    decimal? BudgetMin,
    decimal? BudgetMax);

public sealed record SendRegistrationOtpRequest(string Email);

public sealed record SendRegistrationOtpResponse(string Email, DateTime ExpiresAtUtc, string? DevelopmentOtp);

public sealed record LoginRequest(string Email, string Password);

public sealed record GoogleLoginRequest(string IdToken);

public sealed record AuthResponse(string Token, DateTime ExpiresAtUtc, UserProfileDto User);

public sealed record UserProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    Gender Gender,
    int? Age,
    decimal? Height,
    decimal? Weight,
    string? ClothingSize,
    string? PreferredStyle,
    decimal? BudgetMin,
    decimal? BudgetMax);

public sealed record UpdateProfileRequest(
    string FirstName,
    string LastName,
    Gender Gender,
    int? Age,
    decimal? Height,
    decimal? Weight,
    string? ClothingSize,
    string? PreferredStyle,
    decimal? BudgetMin,
    decimal? BudgetMax);
