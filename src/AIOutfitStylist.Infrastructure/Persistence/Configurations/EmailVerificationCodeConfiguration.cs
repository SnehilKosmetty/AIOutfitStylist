using AIOutfitStylist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIOutfitStylist.Infrastructure.Persistence.Configurations;

public sealed class EmailVerificationCodeConfiguration : IEntityTypeConfiguration<EmailVerificationCode>
{
    public void Configure(EntityTypeBuilder<EmailVerificationCode> builder)
    {
        builder.ToTable("EmailVerificationCodes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.Property(x => x.CodeHash).HasMaxLength(512).IsRequired();
        builder.HasIndex(x => new { x.Email, x.ExpiresAtUtc });
    }
}
