using AIOutfitStylist.Application.DTOs;
using FluentValidation;

namespace AIOutfitStylist.Application.Validation;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(80);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256).Must(HaveRealDomain).WithMessage("Enter a valid email with a real domain.");
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .MaximumLength(128)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        RuleFor(x => x.OtpCode).NotEmpty().Length(6).Matches("^[0-9]{6}$").WithMessage("Enter the 6 digit OTP code.");
        RuleFor(x => x.Age).InclusiveBetween(13, 120).When(x => x.Age.HasValue);
        RuleFor(x => x.BudgetMax).GreaterThanOrEqualTo(x => x.BudgetMin).When(x => x.BudgetMin.HasValue && x.BudgetMax.HasValue);
    }

    private static bool HaveRealDomain(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2)
        {
            return false;
        }

        var domain = parts[1];
        return domain.Contains('.') && !domain.EndsWith(".test", StringComparison.OrdinalIgnoreCase) && !domain.EndsWith(".invalid", StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class SendRegistrationOtpRequestValidator : AbstractValidator<SendRegistrationOtpRequest>
{
    public SendRegistrationOtpRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
    }
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class GoogleLoginRequestValidator : AbstractValidator<GoogleLoginRequest>
{
    public GoogleLoginRequestValidator()
    {
        RuleFor(x => x.IdToken).NotEmpty();
    }
}
