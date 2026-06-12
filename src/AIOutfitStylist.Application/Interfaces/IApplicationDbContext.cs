using AIOutfitStylist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIOutfitStylist.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<UserPhoto> UserPhotos { get; }
    DbSet<PhotoAnalysis> PhotoAnalyses { get; }
    DbSet<Outfit> Outfits { get; }
    DbSet<OutfitItem> OutfitItems { get; }
    DbSet<Product> Products { get; }
    DbSet<SavedOutfit> SavedOutfits { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
