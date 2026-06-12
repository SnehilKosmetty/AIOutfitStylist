using AIOutfitStylist.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIOutfitStylist.Api.Controllers;

[Authorize]
public sealed class PhotosController(IPhotoService photoService) : ApiControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(15 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 15 * 1024 * 1024)]
    public async Task<IActionResult> Upload(IFormFile? image, CancellationToken cancellationToken)
    {
        if (image is null)
        {
            return BadRequest(new { message = "Please select an image file." });
        }

        await using var stream = image.OpenReadStream();
        var result = await photoService.UploadAsync(UserId, stream, image.FileName, image.ContentType, image.Length, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }
}
