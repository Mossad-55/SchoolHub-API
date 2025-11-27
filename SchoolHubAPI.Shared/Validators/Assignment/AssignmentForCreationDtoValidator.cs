using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Assignment;

namespace SchoolHubAPI.Shared.Validators.Assignment;

public class AssignmentForCreationDtoValidator : AbstractValidator<AssignmentForCreationDto>
{
    public AssignmentForCreationDtoValidator()
    {
        RuleFor(a => a.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

        RuleFor(a => a.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(a => a.DueDate)
            .NotNull().WithMessage("Due date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");

        RuleFor(a => a.CreatedDate)
            .NotNull().WithMessage("Created date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Created date cannot be in the future.");
    }
}
