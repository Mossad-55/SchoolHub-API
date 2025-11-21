using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Course;

namespace SchoolHubAPI.Shared.Validators.Courses;

public class CourseForUpdateDtoValidator : AbstractValidator<CourseForUpdateDto>
{
    public CourseForUpdateDtoValidator()
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
        RuleFor(x => x.UpdatedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.UpdatedDate.HasValue)
            .WithMessage("UpdatedDate cannot be in the future.");
    }
}
