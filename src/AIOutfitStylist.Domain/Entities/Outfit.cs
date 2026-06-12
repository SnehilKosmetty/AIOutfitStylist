using AIOutfitStylist.Domain.Common;
using AIOutfitStylist.Domain.Enums;

namespace AIOutfitStylist.Domain.Entities;

public sealed class Outfit : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public Guid? PhotoAnalysisId { get; set; }
    public PhotoAnalysis? PhotoAnalysis { get; set; }
    public string Name { get; set; } = string.Empty;
    public Occasion Occasion { get; set; }
    public decimal Budget { get; set; }
    public string Weather { get; set; } = string.Empty;
    public string StylePreference { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    public string StylingExplanation { get; set; } = string.Empty;
    public string? GeneratedImageUrl { get; set; }
    public ICollection<OutfitItem> Items { get; set; } = new List<OutfitItem>();
    public ICollection<SavedOutfit> SavedByUsers { get; set; } = new List<SavedOutfit>();
}
