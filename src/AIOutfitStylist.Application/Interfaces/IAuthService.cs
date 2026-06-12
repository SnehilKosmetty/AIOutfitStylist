using AIOutfitStylist.Application.Common;
using AIOutfitStylist.Application.DTOs;

namespace AIOutfitStylist.Application.Interfaces;

public interface IAuthService
{
    Task<Result<SendRegistrationOtpResponse>> SendRegistrationOtpAsync(SendRegistrationOtpRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GoogleLoginAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default);
    Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
}
