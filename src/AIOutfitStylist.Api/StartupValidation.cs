using AIOutfitStylist.Infrastructure.Options;

namespace AIOutfitStylist.Api;

public static class StartupValidation
{
    public static void ValidateProductionConfiguration(this WebApplication app)
    {
        var configuration = app.Configuration;
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var admin = configuration.GetSection(AdminOptions.SectionName).Get<AdminOptions>() ?? new AdminOptions();
        var email = configuration.GetSection(EmailOptions.SectionName).Get<EmailOptions>() ?? new EmailOptions();
        var openAi = configuration.GetSection(OpenAiOptions.SectionName).Get<OpenAiOptions>() ?? new OpenAiOptions();
        var google = configuration.GetSection(GoogleAuthOptions.SectionName).Get<GoogleAuthOptions>() ?? new GoogleAuthOptions();
        var blob = configuration.GetSection(AzureBlobOptions.SectionName).Get<AzureBlobOptions>() ?? new AzureBlobOptions();

        if (string.IsNullOrWhiteSpace(jwt.SigningKey) ||
            jwt.SigningKey.StartsWith("CHANGE_ME", StringComparison.OrdinalIgnoreCase) ||
            jwt.SigningKey.Length < 64)
        {
            throw new InvalidOperationException("Jwt:SigningKey must be a real random secret with at least 64 characters.");
        }

        if (admin.AllowedEmails.Length == 0)
        {
            throw new InvalidOperationException("Admin:AllowedEmails must contain at least one admin email.");
        }

        if (!email.IsConfigured)
        {
            throw new InvalidOperationException("Email SMTP settings must be configured because registration OTP delivery requires email.");
        }

        if (string.IsNullOrWhiteSpace(openAi.ApiKey))
        {
            throw new InvalidOperationException("OpenAI:ApiKey must be configured.");
        }

        if (string.IsNullOrWhiteSpace(google.ClientId))
        {
            throw new InvalidOperationException("GoogleAuth:ClientId must be configured.");
        }

        if (string.IsNullOrWhiteSpace(blob.ConnectionString))
        {
            throw new InvalidOperationException("AzureBlobStorage:ConnectionString must be configured.");
        }
    }
}
