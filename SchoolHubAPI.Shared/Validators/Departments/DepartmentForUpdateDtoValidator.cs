using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Department;

namespace SchoolHubAPI.Shared.Validators.Departments;

public class DepartmentForUpdateDtoValidator : AbstractValidator<DepartmentForUpdateDto>
{
    public DepartmentForUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name cannot be whitespace.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.UpdatedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.UpdatedDate.HasValue)
            .WithMessage("UpdatedDate cannot be in the future.");

        RuleFor(x => x.HeadOfDepartmentId)
            .Must(id => id.HasValue && id.Value != Guid.Empty)
            .When(x => x.HeadOfDepartmentId.HasValue)
            .WithMessage("HeadOfDepartmentId, when provided, must be a valid non-empty GUID.");
    }
}
