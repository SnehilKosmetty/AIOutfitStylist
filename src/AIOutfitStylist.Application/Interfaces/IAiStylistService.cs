using AIOutfitStylist.Application.DTOs;

namespace AIOutfitStylist.Application.Interfaces;

public interface IAiStylistService
{
    Task<PhotoAnalysisDto> AnalyzePhotoAsync(Guid userId, Guid photoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutfitDto>> GenerateOutfitsAsync(Guid userId, GenerateOutfitRequest request, CancellationToken cancellationToken = default);
}
