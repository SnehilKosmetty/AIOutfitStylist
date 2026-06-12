using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AIOutfitStylist.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User id missing."));
}
