using AIOutfitStylist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIOutfitStylist.Infrastructure.Persistence.Configurations;

public sealed class PhotoAnalysisConfiguration : IEntityTypeConfiguration<PhotoAnalysis>
{
    public void Configure(EntityTypeBuilder<PhotoAnalysis> builder)
    {
        builder.ToTable("PhotoAnalysis");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BodyType).HasMaxLength(120).IsRequired();
        builder.Property(x => x.SkinTone).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Style).HasMaxLength(120).IsRequired();
        builder.Property(x => x.RecommendedColorsJson).IsRequired();
        builder.Property(x => x.RecommendationsJson).IsRequired();
        builder.Property(x => x.RecommendedCategoriesJson).IsRequired();
        builder.Property(x => x.RawAiResponseJson).IsRequired();
        builder.HasOne(x => x.UserPhoto).WithOne(x => x.Analysis).HasForeignKey<PhotoAnalysis>(x => x.UserPhotoId).OnDelete(DeleteBehavior.Cascade);
    }
}
