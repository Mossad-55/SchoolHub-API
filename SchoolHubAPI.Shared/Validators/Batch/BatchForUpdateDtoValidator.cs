using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Batch;

namespace SchoolHubAPI.Shared.Validators.Batch;

public class BatchForUpdateDtoValidator : AbstractValidator<BatchForUpdateDto>
{
    public BatchForUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .Must(name => !string.IsNullOrWhiteSpace(name?.Trim())).WithMessage("Name cannot be whitespace.");

        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester is required.")
            .MaximumLength(50).WithMessage("Semester must not exceed 50 characters.")
            .Must(s => !string.IsNullOrWhiteSpace(s?.Trim())).WithMessage("Semester cannot be whitespace.");

        RuleFor(x => x.StartDate)
            .NotNull().WithMessage("StartDate is required.")
            .Must((dto, start) =>
            {
                if (start is null) return false;
                return dto.EndDate is null || start.Value <= dto.EndDate.Value;
            }).WithMessage("StartDate must be on or before EndDate when EndDate is provided.");

        RuleFor(x => x.EndDate)
            .NotNull().WithMessage("EndDate is required.")
            .Must((dto, end) =>
            {
                if (end is null) return false;
                return dto.StartDate is null || end.Value >= dto.StartDate.Value;
            }).WithMessage("EndDate must be on or after StartDate when StartDate is provided.");

        RuleFor(x => x.UpdatedDate)
            .Must(ud => ud is null || ud.Value <= DateTime.UtcNow)
            .WithMessage("UpdatedDate cannot be in the future.");
    }
}
