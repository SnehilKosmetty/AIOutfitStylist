using AIOutfitStylist.Application.DTOs;
using AIOutfitStylist.Application.Interfaces;

namespace AIOutfitStylist.Infrastructure.Services;

public sealed class ProductSearchService : IProductSearchService
{
    private static readonly string[] Retailers =
    [
        "Amazon",
        "Walmart",
        "Target",
        "H&M",
        "Old Navy",
        "Gap",
        "Macy's",
        "Nordstrom Rack",
        "Zara",
        "ASOS"
    ];

    public Task<IReadOnlyList<ProductDto>> SearchAsync(string category, string style, decimal maxPrice, string department, string color, CancellationToken cancellationToken = default)
    {
        var normalizedColor = NormalizeColor(color);
        var normalizedCategory = NormalizeCategory(category);
        var displayCategory = NormalizeDisplayCategory(category);
        var productQuery = $"{department} {normalizedColor} {normalizedCategory} under ${Math.Ceiling(maxPrice)}";
        var displayName = $"{ToTitleCase(department)} {normalizedColor} {displayCategory}";
        var perRetailer = Retailers.Select((retailer, index) =>
        {
            var priceFactor = 0.58m + (index % 4 * 0.09m);
            var price = Math.Max(8, Math.Round(maxPrice * priceFactor, 2));
            return new ProductDto(
                Guid.NewGuid(),
                displayName,
                retailer,
                Math.Min(price, maxPrice),
                4.6m - (index % 3 * 0.1m),
                BuildSearchLink(retailer, productQuery),
                $"https://placehold.co/640x800/png?text={Uri.EscapeDataString(retailer + " " + normalizedColor + " " + displayCategory)}",
                retailer,
                category);
        }).ToList();

        return Task.FromResult<IReadOnlyList<ProductDto>>(perRetailer);
    }

    private static string NormalizeCategory(string category) => category switch
    {
        "Shirt/Top" => "shirt top",
        "Bottom" => "pants jeans chinos trousers",
        "Shoes" => "shoes",
        "Accessories" => "accessories watch belt",
        _ => category
    };

    private static string NormalizeDisplayCategory(string category) => category switch
    {
        "Shirt/Top" => "shirt",
        "Bottom" => "pants",
        "Shoes" => "shoes",
        "Accessories" => "accessory",
        _ => category.ToLowerInvariant()
    };

    private static string NormalizeColor(string color) => color
        .Replace("earth tones", "brown", StringComparison.OrdinalIgnoreCase)
        .Replace("muted blues", "blue", StringComparison.OrdinalIgnoreCase)
        .Replace("grays", "gray", StringComparison.OrdinalIgnoreCase)
        .Replace("deep red", "burgundy", StringComparison.OrdinalIgnoreCase)
        .Replace("charcoal", "dark gray", StringComparison.OrdinalIgnoreCase)
        .Trim()
        .ToLowerInvariant();

    private static string ToTitleCase(string value) => string.Join(' ', value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => char.ToUpperInvariant(word[0]) + word[1..]));

    private static string BuildSearchLink(string retailer, string query)
    {
        var encoded = Uri.EscapeDataString(query);
        return retailer switch
        {
            "Amazon" => $"https://www.amazon.com/s?k={encoded}",
            "Walmart" => $"https://www.walmart.com/search?q={encoded}",
            "Target" => $"https://www.target.com/s?searchTerm={encoded}",
            "H&M" => $"https://www2.hm.com/en_us/search-results.html?q={encoded}",
            "Old Navy" => $"https://oldnavy.gap.com/browse/search.do?searchText={encoded}",
            "Gap" => $"https://www.gap.com/browse/search.do?searchText={encoded}",
            "Macy's" => $"https://www.macys.com/shop/featured/{encoded}",
            "Nordstrom Rack" => $"https://www.nordstromrack.com/search?keyword={encoded}",
            "Zara" => $"https://www.zara.com/us/en/search?searchTerm={encoded}",
            "ASOS" => $"https://www.asos.com/us/search/?q={encoded}",
            _ => $"https://www.google.com/search?q={encoded}"
        };
    }
}
