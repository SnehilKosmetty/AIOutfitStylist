using AIOutfitStylist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIOutfitStylist.Infrastructure.Persistence.Configurations;

public sealed class SavedOutfitConfiguration : IEntityTypeConfiguration<SavedOutfit>
{
    public void Configure(EntityTypeBuilder<SavedOutfit> builder)
    {
        builder.ToTable("SavedOutfits");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.UserId, x.OutfitId }).IsUnique();
        builder.HasOne(x => x.User).WithMany(x => x.SavedOutfits).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Outfit).WithMany(x => x.SavedByUsers).HasForeignKey(x => x.OutfitId).OnDelete(DeleteBehavior.NoAction);
    }
}
