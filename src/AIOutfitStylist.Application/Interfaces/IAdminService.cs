using AIOutfitStylist.Application.DTOs;

namespace AIOutfitStylist.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}
