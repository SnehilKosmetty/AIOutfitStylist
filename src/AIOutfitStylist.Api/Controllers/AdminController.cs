using AIOutfitStylist.Application.Interfaces;
using AIOutfitStylist.Infrastructure.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AIOutfitStylist.Api.Controllers;

[Authorize]
public sealed class AdminController(IAdminService adminService, IOptions<AdminOptions> options) : ApiControllerBase
{
    private readonly AdminOptions _options = options.Value;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        if (!IsAllowedAdmin())
        {
            return Forbid();
        }

        return Ok(await adminService.GetDashboardAsync(cancellationToken));
    }

    private bool IsAllowedAdmin()
    {
        if (_options.AllowedEmails.Length == 0)
        {
            return true;
        }

        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        return !string.IsNullOrWhiteSpace(email) && _options.AllowedEmails.Any(x => string.Equals(x, email, StringComparison.OrdinalIgnoreCase));
    }
}
