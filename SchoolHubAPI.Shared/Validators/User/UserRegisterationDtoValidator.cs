using System;
using System.Linq;
using FluentValidation;
using SchoolHubAPI.Shared.DTOs.User;

namespace SchoolHubAPI.Shared.Validators.User;

public class UserRegisterationDtoValidator : AbstractValidator<UserRegisterationDto>
{
    private static readonly string[] AllowedRoles = new[] { "Admin", "Teacher", "Student" };

    public UserRegisterationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?\d{7,15}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Phone number must contain only digits, may start with '+', and be 7-15 characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one digit.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(r => AllowedRoles.Contains(r))
            .WithMessage($"Role must be one of: {string.Join(", ", AllowedRoles)}.");

        RuleFor(x => x.CreatedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.CreatedDate.HasValue)
            .WithMessage("CreatedDate cannot be in the future.");
    }
}
