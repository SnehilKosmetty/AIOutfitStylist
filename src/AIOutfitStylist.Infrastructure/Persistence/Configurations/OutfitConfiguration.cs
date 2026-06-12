using AIOutfitStylist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIOutfitStylist.Infrastructure.Persistence.Configurations;

public sealed class OutfitConfiguration : IEntityTypeConfiguration<Outfit>
{
    public void Configure(EntityTypeBuilder<Outfit> builder)
    {
        builder.ToTable("Outfits");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Budget).HasPrecision(10, 2);
        builder.Property(x => x.EstimatedCost).HasPrecision(10, 2);
        builder.Property(x => x.Weather).HasMaxLength(80).IsRequired();
        builder.Property(x => x.StylePreference).HasMaxLength(120).IsRequired();
        builder.Property(x => x.StylingExplanation).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.GeneratedImageUrl).HasMaxLength(1000);
        builder.HasOne(x => x.User).WithMany(x => x.Outfits).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.PhotoAnalysis).WithMany().HasForeignKey(x => x.PhotoAnalysisId).OnDelete(DeleteBehavior.SetNull);
    }
}
