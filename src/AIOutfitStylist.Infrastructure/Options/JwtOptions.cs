namespace AIOutfitStylist.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = "AIOutfitStylist";
    public string Audience { get; set; } = "AIOutfitStylist";
    public string SigningKey { get; set; } = "CHANGE_ME_TO_A_64_CHARACTER_SECRET_IN_PRODUCTION";
    public int ExpirationMinutes { get; set; } = 120;
}
