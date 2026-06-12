using Azure.Storage.Blobs;
using AIOutfitStylist.Application.Interfaces;
using AIOutfitStylist.Infrastructure.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace AIOutfitStylist.Infrastructure.Services;

public sealed class AzureBlobStorageService(IOptions<AzureBlobOptions> options, IWebHostEnvironment environment) : IStorageService
{
    private readonly AzureBlobOptions _options = options.Value;

    public async Task<(string BlobName, string Url)> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            var safeFileName = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
            var uploadRoot = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "uploads");
            Directory.CreateDirectory(uploadRoot);

            var filePath = Path.Combine(uploadRoot, safeFileName);
            await using var fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream, cancellationToken);

            return ($"uploads/{safeFileName}", $"/uploads/{safeFileName}");
        }

        var container = new BlobContainerClient(_options.ConnectionString, _options.ContainerName);
        await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blobName = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}-{Path.GetFileName(fileName)}";
        var blob = container.GetBlobClient(blobName);
        await blob.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
        return (blobName, blob.Uri.ToString());
    }
}
