using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AIOutfitStylist.Application.Common;
using AIOutfitStylist.Application.DTOs;
using AIOutfitStylist.Application.Interfaces;
using AIOutfitStylist.Domain.Entities;
using AIOutfitStylist.Infrastructure.Options;
using AIOutfitStylist.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace AIOutfitStylist.Infrastructure.Services;

public sealed class AuthService(
    ApplicationDbContext dbContext,
    IOptions<JwtOptions> jwtOptions,
    IOptions<GoogleAuthOptions> googleOptions,
    IEmailSender emailSender,
    IHttpClientFactory httpClientFactory) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly GoogleAuthOptions _googleOptions = googleOptions.Value;

    public async Task<Result<SendRegistrationOtpResponse>> SendRegistrationOtpAsync(SendRegistrationOtpRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await dbContext.Users.AnyAsync(x => x.Email == normalizedEmail, cancellationToken))
        {
            return Result<SendRegistrationOtpResponse>.Failure("An account already exists for this email.");
        }

        var code = Random.Shared.Next(100000, 999999).ToString();
        var verification = new EmailVerificationCode
        {
            Email = normalizedEmail,
            CodeHash = BCrypt.Net.BCrypt.HashPassword(code),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5)
        };

        await dbContext.EmailVerificationCodes.AddAsync(verification, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var sent = await emailSender.SendAsync(
            normalizedEmail,
            "Your AI Outfit Stylist verification code",
            $"Your AI Outfit Stylist OTP is {code}. It is valid for 5 minutes.",
            cancellationToken);

        return Result<SendRegistrationOtpResponse>.Success(new SendRegistrationOtpResponse(normalizedEmail, verification.ExpiresAtUtc, sent ? null : code));
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await dbContext.Users.AnyAsync(x => x.Email == normalizedEmail, cancellationToken))
        {
            return Result<AuthResponse>.Failure("An account already exists for this email.");
        }

        var verification = await dbContext.EmailVerificationCodes
            .Where(x => x.Email == normalizedEmail && x.UsedAtUtc == null && x.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (verification is null || !BCrypt.Net.BCrypt.Verify(request.OtpCode.Trim(), verification.CodeHash))
        {
            return Result<AuthResponse>.Failure("Invalid or expired OTP code.");
        }

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Gender = request.Gender,
            Age = request.Age,
            Height = request.Height,
            Weight = request.Weight,
            ClothingSize = request.ClothingSize,
            PreferredStyle = request.PreferredStyle,
            BudgetMin = request.BudgetMin,
            BudgetMax = request.BudgetMax
        };

        verification.UsedAtUtc = DateTime.UtcNow;
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result<AuthResponse>.Success(CreateAuthResponse(user));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure("Invalid email or password.");
        }

        return Result<AuthResponse>.Success(CreateAuthResponse(user));
    }

    public async Task<Result<AuthResponse>> GoogleLoginAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_googleOptions.ClientId))
        {
            return Result<AuthResponse>.Failure("Google login is not configured.");
        }

        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={Uri.EscapeDataString(request.IdToken)}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Result<AuthResponse>.Failure("Google login token is invalid.");
        }

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        var root = payload.RootElement;
        var audience = root.TryGetProperty("aud", out var aud) ? aud.GetString() : null;
        var email = root.TryGetProperty("email", out var emailElement) ? emailElement.GetString() : null;
        var verified = root.TryGetProperty("email_verified", out var verifiedElement) && verifiedElement.GetString() == "true";

        if (audience != _googleOptions.ClientId || string.IsNullOrWhiteSpace(email) || !verified)
        {
            return Result<AuthResponse>.Failure("Google account could not be verified.");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
        if (user is null)
        {
            var givenName = root.TryGetProperty("given_name", out var given) ? given.GetString() : null;
            var familyName = root.TryGetProperty("family_name", out var family) ? family.GetString() : null;
            var name = root.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : normalizedEmail.Split('@')[0];

            user = new User
            {
                FirstName = string.IsNullOrWhiteSpace(givenName) ? name?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "Google" : givenName,
                LastName = string.IsNullOrWhiteSpace(familyName) ? "User" : familyName,
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N"))
            };

            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result<AuthResponse>.Success(CreateAuthResponse(user));
    }

    public async Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        return user?.ToProfileDto();
    }

    public async Task<Result<UserProfileDto>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
        {
            return Result<UserProfileDto>.Failure("User not found.");
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Gender = request.Gender;
        user.Age = request.Age;
        user.Height = request.Height;
        user.Weight = request.Weight;
        user.ClothingSize = request.ClothingSize;
        user.PreferredStyle = request.PreferredStyle;
        user.BudgetMin = request.BudgetMin;
        user.BudgetMax = request.BudgetMax;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result<UserProfileDto>.Success(user.ToProfileDto());
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token), expires, user.ToProfileDto());
    }
}
