using AIOutfitStylist.Application.Common;
using AIOutfitStylist.Application.DTOs;

namespace AIOutfitStylist.Application.Interfaces;

public interface IOutfitService
{
    Task<IReadOnlyList<OutfitDto>> GenerateAsync(Guid userId, GenerateOutfitRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutfitDto>> GetHistoryAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<OutfitDto>> SaveAsync(Guid userId, SaveOutfitRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid userId, Guid outfitId, CancellationToken cancellationToken = default);
}
