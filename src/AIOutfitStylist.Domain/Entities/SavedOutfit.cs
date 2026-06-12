using AIOutfitStylist.Domain.Common;

namespace AIOutfitStylist.Domain.Entities;

public sealed class SavedOutfit : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public Guid OutfitId { get; set; }
    public Outfit Outfit { get; set; } = default!;
    public string? Notes { get; set; }
}
