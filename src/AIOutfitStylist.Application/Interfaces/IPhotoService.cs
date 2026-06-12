using AIOutfitStylist.Application.Common;
using AIOutfitStylist.Application.DTOs;

namespace AIOutfitStylist.Application.Interfaces;

public interface IPhotoService
{
    Task<Result<PhotoUploadResponse>> UploadAsync(Guid userId, Stream stream, string fileName, string contentType, long sizeBytes, CancellationToken cancellationToken = default);
}
