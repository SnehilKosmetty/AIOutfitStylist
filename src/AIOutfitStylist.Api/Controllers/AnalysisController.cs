using AIOutfitStylist.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIOutfitStylist.Api.Controllers;

[Authorize]
public sealed class AnalysisController(IAiStylistService aiStylistService) : ApiControllerBase
{
    [HttpPost("analyze-photo")]
    public async Task<IActionResult> AnalyzePhoto([FromBody] AnalyzePhotoRequest request, CancellationToken cancellationToken)
    {
        var analysis = await aiStylistService.AnalyzePhotoAsync(UserId, request.PhotoId, cancellationToken);
        return Ok(analysis);
    }
}

public sealed record AnalyzePhotoRequest(Guid PhotoId);
