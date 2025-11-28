using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Submission;

namespace SchoolHubAPI.Shared.Validators.Submission;

public class SubmissionForCreationDtoValidator : AbstractValidator<SubmissionForCreationDto>
{
    private readonly string[] allowedExtensions = new[] { ".pdf", ".doc", ".docx" };

    public SubmissionForCreationDtoValidator()
    {
        RuleFor(x => x.SubmittedDate)
            .NotNull().WithMessage("Submitted date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Submitted date cannot be in the future.");

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("A file must be provided.")
            .Must(file => file!.Length > 0)
            .WithMessage("The file cannot be empty.")
            .Must(file => allowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
            .WithMessage($"Only the following file types are allowed: {string.Join(", ", allowedExtensions)}");
    }
}
