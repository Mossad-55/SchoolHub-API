using FluentValidation;
using Microsoft.AspNetCore.Http;
using SchoolHubAPI.Shared.DTOs.Submission;
using System.IO;

namespace SchoolHubAPI.Shared.Validators.Submission
{
    public class SubmissionForUpdateDtoValidator : AbstractValidator<SubmissionForUpdateDto>
    {
        private readonly string[] allowedExtensions = new[] { ".pdf", ".doc", ".docx" };

        public SubmissionForUpdateDtoValidator()
        {
            RuleFor(x => x.SubmittedDate)
                .NotNull()
                .WithMessage("Submitted date is required.")
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
}
