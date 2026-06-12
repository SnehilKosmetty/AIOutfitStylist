namespace AIOutfitStylist.Application.Interfaces;

public interface IStorageService
{
    Task<(string BlobName, string Url)> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);
}
