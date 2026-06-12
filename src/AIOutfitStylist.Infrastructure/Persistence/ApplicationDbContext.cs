using AIOutfitStylist.Application.Interfaces;
using AIOutfitStylist.Domain.Common;
using AIOutfitStylist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIOutfitStylist.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserPhoto> UserPhotos => Set<UserPhoto>();
    public DbSet<PhotoAnalysis> PhotoAnalyses => Set<PhotoAnalysis>();
    public DbSet<Outfit> Outfits => Set<Outfit>();
    public DbSet<OutfitItem> OutfitItems => Set<OutfitItem>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SavedOutfit> SavedOutfits => Set<SavedOutfit>();
    public DbSet<EmailVerificationCode> EmailVerificationCodes => Set<EmailVerificationCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
