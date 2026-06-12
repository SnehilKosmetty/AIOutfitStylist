using AIOutfitStylist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIOutfitStylist.Infrastructure.Persistence.Configurations;

public sealed class UserPhotoConfiguration : IEntityTypeConfiguration<UserPhoto>
{
    public void Configure(EntityTypeBuilder<UserPhoto> builder)
    {
        builder.ToTable("UserPhotos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(80).IsRequired();
        builder.Property(x => x.BlobName).HasMaxLength(512).IsRequired();
        builder.Property(x => x.BlobUrl).HasMaxLength(1000).IsRequired();
        builder.HasOne(x => x.User).WithMany(x => x.Photos).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
