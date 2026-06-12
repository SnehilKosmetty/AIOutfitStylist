using AIOutfitStylist.Application.DTOs;
using AIOutfitStylist.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIOutfitStylist.Api.Controllers;

[Authorize]
public sealed class OutfitsController(IOutfitService outfitService, IValidator<GenerateOutfitRequest> generateValidator) : ApiControllerBase
{
    [HttpPost("generate")]
    public async Task<IActionResult> Generate(GenerateOutfitRequest request, CancellationToken cancellationToken)
    {
        var validation = await generateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var problemDetails = new ValidationProblemDetails();
            foreach (var failure in validation.Errors)
            {
                if (!problemDetails.Errors.ContainsKey(failure.PropertyName))
                {
                    problemDetails.Errors[failure.PropertyName] = [];
                }
                problemDetails.Errors[failure.PropertyName] = problemDetails.Errors[failure.PropertyName].Append(failure.ErrorMessage).ToArray();
            }
            return new BadRequestObjectResult(problemDetails);
        }

        var outfits = await outfitService.GenerateAsync(UserId, request, cancellationToken);
        return Ok(outfits);
    }

    [HttpGet("history")]
    public async Task<IActionResult> History(CancellationToken cancellationToken)
    {
        var outfits = await outfitService.GetHistoryAsync(UserId, cancellationToken);
        return Ok(outfits);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(SaveOutfitRequest request, CancellationToken cancellationToken)
    {
        var result = await outfitService.SaveAsync(UserId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await outfitService.DeleteAsync(UserId, id, cancellationToken);
        return result.Succeeded ? NoContent() : NotFound(new { message = result.Error });
    }
}
