using FluentValidation;
using SchoolHubAPI.Shared.DTOs.User;

namespace SchoolHubAPI.Shared.Validators;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        // Name: optional, but if provided cannot be empty and has a max length
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().When(x => x.Name is not null).WithMessage("Name cannot be empty.")
            .MaximumLength(100).When(x => x.Name is not null).WithMessage("Name must not exceed 100 characters.");

        // PhoneNumber: optional, validate pattern if present
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?\d{7,15}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Phone number must contain only digits, may start with '+', and be 7-15 characters long.");

        // UpdatedDate: optional, cannot be in the future
        RuleFor(x => x.UpdatedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.UpdatedDate.HasValue)
            .WithMessage("UpdatedDate cannot be in the future.");
    }
}
