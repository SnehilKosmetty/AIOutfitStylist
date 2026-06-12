using AIOutfitStylist.Domain.Common;

namespace AIOutfitStylist.Domain.Entities;

public sealed class Product : BaseEntity
{
    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Rating { get; set; }
    public string PurchaseLink { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public string Retailer { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
