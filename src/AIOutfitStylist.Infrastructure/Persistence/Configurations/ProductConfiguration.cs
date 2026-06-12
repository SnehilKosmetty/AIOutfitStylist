using AIOutfitStylist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIOutfitStylist.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductName).HasMaxLength(220).IsRequired();
        builder.Property(x => x.Brand).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Price).HasPrecision(10, 2);
        builder.Property(x => x.Rating).HasPrecision(3, 2);
        builder.Property(x => x.PurchaseLink).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.ProductImage).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Retailer).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(80).IsRequired();
    }
}
