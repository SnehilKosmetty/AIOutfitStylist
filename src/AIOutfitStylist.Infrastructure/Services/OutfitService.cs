using AIOutfitStylist.Application.Common;
using AIOutfitStylist.Application.DTOs;
using AIOutfitStylist.Application.Interfaces;
using AIOutfitStylist.Domain.Entities;
using AIOutfitStylist.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIOutfitStylist.Infrastructure.Services;

public sealed class OutfitService(ApplicationDbContext dbContext, IAiStylistService aiStylistService) : IOutfitService
{
    public Task<IReadOnlyList<OutfitDto>> GenerateAsync(Guid userId, GenerateOutfitRequest request, CancellationToken cancellationToken = default) =>
        aiStylistService.GenerateOutfitsAsync(userId, request, cancellationToken);

    public async Task<IReadOnlyList<OutfitDto>> GetHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var savedIds = await dbContext.SavedOutfits.AsNoTracking().Where(x => x.UserId == userId).Select(x => x.OutfitId).ToListAsync(cancellationToken);
        var outfits = await dbContext.Outfits
            .AsNoTracking()
            .Include(x => x.Items).ThenInclude(x => x.Product)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return outfits.Select(x => x.ToDto(savedIds.Contains(x.Id))).ToList();
    }

    public async Task<Result<OutfitDto>> SaveAsync(Guid userId, SaveOutfitRequest request, CancellationToken cancellationToken = default)
    {
        var outfit = await dbContext.Outfits.Include(x => x.Items).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.Id == request.OutfitId && x.UserId == userId, cancellationToken);
        if (outfit is null)
        {
            return Result<OutfitDto>.Failure("Outfit not found.");
        }

        if (!await dbContext.SavedOutfits.AnyAsync(x => x.UserId == userId && x.OutfitId == request.OutfitId, cancellationToken))
        {
            await dbContext.SavedOutfits.AddAsync(new SavedOutfit { UserId = userId, OutfitId = request.OutfitId, Notes = request.Notes }, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result<OutfitDto>.Success(outfit.ToDto(true));
    }

    public async Task<Result<bool>> DeleteAsync(Guid userId, Guid outfitId, CancellationToken cancellationToken = default)
    {
        var saved = await dbContext.SavedOutfits.FirstOrDefaultAsync(x => x.UserId == userId && x.OutfitId == outfitId, cancellationToken);
        if (saved is not null)
        {
            dbContext.SavedOutfits.Remove(saved);
        }

        var outfit = await dbContext.Outfits.FirstOrDefaultAsync(x => x.Id == outfitId && x.UserId == userId, cancellationToken);
        if (outfit is null && saved is null)
        {
            return Result<bool>.Failure("Outfit not found.");
        }

        if (outfit is not null)
        {
            dbContext.Outfits.Remove(outfit);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
