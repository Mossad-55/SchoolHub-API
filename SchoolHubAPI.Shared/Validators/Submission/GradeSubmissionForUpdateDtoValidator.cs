using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Submission;

namespace SchoolHubAPI.Shared.Validators.Submission;

public class GradeSubmissionForUpdateDtoValidator : AbstractValidator<GradeSubmissionForUpdateDto>
{
    public GradeSubmissionForUpdateDtoValidator()
    {
        RuleFor(x => x.Grade)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Grade cannot be less than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Grade cannot exceed 100.");

        RuleFor(x => x.Remarks)
            .MaximumLength(500)
            .WithMessage("Remarks cannot exceed 500 characters.");

        RuleFor(x => x.Remarks)
            .NotEmpty()
            .WithMessage("Remarks are required when the grade is below 50.")
            .When(x => x.Grade < 50);
    }
}
