using AIOutfitStylist.Domain.Common;
using AIOutfitStylist.Domain.Enums;

namespace AIOutfitStylist.Domain.Entities;

public sealed class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.NotSpecified;
    public int? Age { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public string? ClothingSize { get; set; }
    public string? PreferredStyle { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public ICollection<UserPhoto> Photos { get; set; } = new List<UserPhoto>();
    public ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();
    public ICollection<SavedOutfit> SavedOutfits { get; set; } = new List<SavedOutfit>();
}
