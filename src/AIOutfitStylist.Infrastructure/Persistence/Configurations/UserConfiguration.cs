using AIOutfitStylist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIOutfitStylist.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirstName).HasMaxLength(80).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(x => x.ClothingSize).HasMaxLength(40);
        builder.Property(x => x.PreferredStyle).HasMaxLength(120);
        builder.Property(x => x.Height).HasPrecision(6, 2);
        builder.Property(x => x.Weight).HasPrecision(6, 2);
        builder.Property(x => x.BudgetMin).HasPrecision(10, 2);
        builder.Property(x => x.BudgetMax).HasPrecision(10, 2);
    }
}
