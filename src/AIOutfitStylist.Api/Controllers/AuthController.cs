using AIOutfitStylist.Application.DTOs;
using AIOutfitStylist.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIOutfitStylist.Api.Controllers;

public sealed class AuthController(
    IAuthService authService,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator,
    IValidator<GoogleLoginRequest> googleLoginValidator,
    IValidator<SendRegistrationOtpRequest> otpValidator) : ApiControllerBase
{
    [HttpPost("send-registration-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendRegistrationOtp(SendRegistrationOtpRequest request, CancellationToken cancellationToken)
    {
        var validation = await otpValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var problemDetails = new ValidationProblemDetails();
            foreach (var failure in validation.Errors)
            {
                if (!problemDetails.Errors.ContainsKey(failure.PropertyName))
                {
                    problemDetails.Errors[failure.PropertyName] = [];
                }
                problemDetails.Errors[failure.PropertyName] = problemDetails.Errors[failure.PropertyName].Append(failure.ErrorMessage).ToArray();
            }
            return new BadRequestObjectResult(problemDetails);
        }

        var result = await authService.SendRegistrationOtpAsync(request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var validation = await registerValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var problemDetails = new ValidationProblemDetails();
            foreach (var failure in validation.Errors)
            {
                if (!problemDetails.Errors.ContainsKey(failure.PropertyName))
                {
                    problemDetails.Errors[failure.PropertyName] = [];
                }
                problemDetails.Errors[failure.PropertyName] = problemDetails.Errors[failure.PropertyName].Append(failure.ErrorMessage).ToArray();
            }
            return new BadRequestObjectResult(problemDetails);
        }

        var result = await authService.RegisterAsync(request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var validation = await loginValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var problemDetails = new ValidationProblemDetails();
            foreach (var failure in validation.Errors)
            {
                if (!problemDetails.Errors.ContainsKey(failure.PropertyName))
                {
                    problemDetails.Errors[failure.PropertyName] = [];
                }
                problemDetails.Errors[failure.PropertyName] = problemDetails.Errors[failure.PropertyName].Append(failure.ErrorMessage).ToArray();
            }
            return new BadRequestObjectResult(problemDetails);
        }

        var result = await authService.LoginAsync(request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Unauthorized(new { message = result.Error });
    }

    [HttpPost("google")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLogin(GoogleLoginRequest request, CancellationToken cancellationToken)
    {
        var validation = await googleLoginValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var problemDetails = new ValidationProblemDetails();
            foreach (var failure in validation.Errors)
            {
                if (!problemDetails.Errors.ContainsKey(failure.PropertyName))
                {
                    problemDetails.Errors[failure.PropertyName] = [];
                }
                problemDetails.Errors[failure.PropertyName] = problemDetails.Errors[failure.PropertyName].Append(failure.ErrorMessage).ToArray();
            }
            return new BadRequestObjectResult(problemDetails);
        }

        var result = await authService.GoogleLoginAsync(request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : Unauthorized(new { message = result.Error });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var profile = await authService.GetProfileAsync(UserId, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.UpdateProfileAsync(UserId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }
}
