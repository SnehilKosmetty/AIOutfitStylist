using AIOutfitStylist.Application.DTOs;
using AIOutfitStylist.Application.Interfaces;
using AIOutfitStylist.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIOutfitStylist.Infrastructure.Services;

public sealed class AdminService(ApplicationDbContext dbContext) : IAdminService
{
    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var recentUsers = await dbContext.Users
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(8)
            .Select(x => new AdminUserActivityDto(
                x.Id,
                x.FirstName + " " + x.LastName,
                x.Email,
                x.Gender.ToString(),
                x.Age,
                x.Height,
                x.Weight,
                x.ClothingSize,
                x.PreferredStyle,
                x.BudgetMin,
                x.BudgetMax,
                x.CreatedAtUtc,
                x.Photos.Count,
                x.Outfits.Count,
                x.SavedOutfits.Count))
            .ToListAsync(cancellationToken);

        return new AdminDashboardDto(
            await dbContext.Users.CountAsync(cancellationToken),
            await dbContext.UserPhotos.CountAsync(cancellationToken),
            await dbContext.PhotoAnalyses.CountAsync(cancellationToken),
            await dbContext.Outfits.CountAsync(cancellationToken),
            await dbContext.SavedOutfits.CountAsync(cancellationToken),
            await dbContext.Outfits.CountAsync(x => x.CreatedAtUtc >= today, cancellationToken),
            recentUsers);
    }
}
