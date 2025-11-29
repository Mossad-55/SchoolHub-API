using FluentValidation;
using Microsoft.AspNetCore.Http;
using SchoolHubAPI.Shared.DTOs.Submission;
using System.IO;

namespace SchoolHubAPI.Shared.Validators.Submission
{
    public class SubmissionForUpdateDtoValidator : AbstractValidator<SubmissionForUpdateDto>
    {
        public SubmissionForUpdateDtoValidator()
        {
            RuleFor(x => x.SubmittedDate)
                .NotNull()
                .WithMessage("Submitted date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Submitted date cannot be in the future.");
        }
    }
}
