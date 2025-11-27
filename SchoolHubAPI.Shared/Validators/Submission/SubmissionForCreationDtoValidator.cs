using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Submission;

namespace SchoolHubAPI.Shared.Validators.Submission;

public class SubmissionForCreationDtoValidator : AbstractValidator<SubmissionForCreationDto>
{
    public SubmissionForCreationDtoValidator()
    {
        RuleFor(x => x.SubmittedDate)
            .NotNull().WithMessage("Submitted date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Submitted date cannot be in the future.");

        RuleFor(x => x.FileUrl)
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.FileUrl))
            .WithMessage("File URL cannot be empty if provided.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.FileUrl))
            .WithMessage("File URL must be a valid absolute URL.");
    }
}
