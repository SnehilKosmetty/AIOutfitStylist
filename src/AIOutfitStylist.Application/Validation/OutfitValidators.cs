using AIOutfitStylist.Application.DTOs;
using FluentValidation;

namespace AIOutfitStylist.Application.Validation;

public sealed class GenerateOutfitRequestValidator : AbstractValidator<GenerateOutfitRequest>
{
    public GenerateOutfitRequestValidator()
    {
        RuleFor(x => x.Budget).GreaterThanOrEqualTo(25).LessThanOrEqualTo(10000);
        RuleFor(x => x.Weather).NotEmpty().MaximumLength(80);
        RuleFor(x => x.StylePreference).NotEmpty().MaximumLength(120);
        RuleFor(x => x.ShoppingDepartment)
            .Must(value => string.IsNullOrWhiteSpace(value) || new[] { "mens", "womens", "unisex" }.Contains(value.Trim().ToLowerInvariant()))
            .WithMessage("Shopping department must be mens, womens, or unisex.");
        RuleFor(x => x.Occasion).IsInEnum();
    }
}
