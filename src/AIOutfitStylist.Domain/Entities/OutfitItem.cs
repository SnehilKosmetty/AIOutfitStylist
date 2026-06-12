using AIOutfitStylist.Domain.Common;

namespace AIOutfitStylist.Domain.Entities;

public sealed class OutfitItem : BaseEntity
{
    public Guid OutfitId { get; set; }
    public Outfit Outfit { get; set; } = default!;
    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal EstimatedPrice { get; set; }
    public string Notes { get; set; } = string.Empty;
}
