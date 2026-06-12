using AIOutfitStylist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIOutfitStylist.Infrastructure.Persistence.Configurations;

public sealed class OutfitItemConfiguration : IEntityTypeConfiguration<OutfitItem>
{
    public void Configure(EntityTypeBuilder<OutfitItem> builder)
    {
        builder.ToTable("OutfitItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Category).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Color).HasMaxLength(80).IsRequired();
        builder.Property(x => x.EstimatedPrice).HasPrecision(10, 2);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasOne(x => x.Outfit).WithMany(x => x.Items).HasForeignKey(x => x.OutfitId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.SetNull);
    }
}
