using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Course;

namespace SchoolHubAPI.Shared.Validators.Courses;

public class CourseForCreationDtoValidator : AbstractValidator<CourseForCreationDto>
{
    public CourseForCreationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Name cannot be whitespace.");
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(10).WithMessage("Code must not exceed 10 characters.")
            .Must(code => !string.IsNullOrWhiteSpace(code)).WithMessage("Code cannot be whitespace.");
        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description must not exceed 2000 characters.");
        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Credits must be greater than zero.");
        RuleFor(x => x.CreatedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.CreatedDate.HasValue)
            .WithMessage("CreatedDate cannot be in the future.");
        RuleFor(x => x.DepartmentId)
            .NotEqual(Guid.Empty).WithMessage("DepartmentId must be a valid non-empty GUID.");
    }
}
