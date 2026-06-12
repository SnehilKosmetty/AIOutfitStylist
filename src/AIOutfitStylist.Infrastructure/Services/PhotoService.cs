using AIOutfitStylist.Application.Common;
using AIOutfitStylist.Application.DTOs;
using AIOutfitStylist.Application.Interfaces;
using AIOutfitStylist.Domain.Entities;
using AIOutfitStylist.Infrastructure.Persistence;

namespace AIOutfitStylist.Infrastructure.Services;

public sealed class PhotoService(ApplicationDbContext dbContext, IStorageService storageService) : IPhotoService
{
    private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png", "image/webp" };
    private const long MaxBytes = 10 * 1024 * 1024;

    public async Task<Result<PhotoUploadResponse>> UploadAsync(Guid userId, Stream stream, string fileName, string contentType, long sizeBytes, CancellationToken cancellationToken = default)
    {
        if (sizeBytes <= 0)
        {
            return Result<PhotoUploadResponse>.Failure("Image is required.");
        }

        if (sizeBytes > MaxBytes)
        {
            return Result<PhotoUploadResponse>.Failure("Image must be 10MB or smaller.");
        }

        if (!AllowedTypes.Contains(contentType))
        {
            return Result<PhotoUploadResponse>.Failure("Supported formats are JPG, PNG, and WEBP.");
        }

        var upload = await storageService.UploadAsync(stream, fileName, contentType, cancellationToken);
        var photo = new UserPhoto
        {
            UserId = userId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            BlobName = upload.BlobName,
            BlobUrl = upload.Url
        };

        await dbContext.UserPhotos.AddAsync(photo, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result<PhotoUploadResponse>.Success(new PhotoUploadResponse(photo.Id, photo.FileName, photo.BlobUrl, photo.SizeBytes, photo.CreatedAtUtc));
    }
}
