using System.Text.RegularExpressions;
using FluentValidation;
using SchoolHubAPI.Shared.DTOs.User;

namespace SchoolHubAPI.Shared.Validators.User;

public class TokenDtoValidator : AbstractValidator<TokenDto>
{
    private static readonly Regex JwtRegex = new(@"^[^.]+\.[^.]+\.[^.]+$");
    private static readonly Regex Base64Regex = new(@"^[A-Za-z0-9\+/=]+$");

    public TokenDtoValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is required.")
            .Matches(JwtRegex).WithMessage("Access token must be a valid JWT (three segments separated by '.').");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .Matches(Base64Regex).WithMessage("Refresh token must be a valid Base64 string.");
    }
}
