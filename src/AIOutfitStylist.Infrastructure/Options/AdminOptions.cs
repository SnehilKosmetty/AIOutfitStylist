namespace AIOutfitStylist.Infrastructure.Options;

public sealed class AdminOptions
{
    public const string SectionName = "Admin";
    public string[] AllowedEmails { get; set; } = [];
}
