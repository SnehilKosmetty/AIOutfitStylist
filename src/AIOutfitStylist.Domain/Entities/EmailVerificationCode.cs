using AIOutfitStylist.Domain.Common;

namespace AIOutfitStylist.Domain.Entities;

public sealed class EmailVerificationCode : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
}
