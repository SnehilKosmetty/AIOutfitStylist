using AIOutfitStylist.Application.DTOs;

namespace AIOutfitStylist.Application.Interfaces;

public interface IProductSearchService
{
    Task<IReadOnlyList<ProductDto>> SearchAsync(string category, string style, decimal maxPrice, string department, string color, CancellationToken cancellationToken = default);
}
