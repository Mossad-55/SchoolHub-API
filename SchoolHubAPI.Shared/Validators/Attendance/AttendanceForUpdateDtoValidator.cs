using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Attendance;

namespace SchoolHubAPI.Shared.Validators.Attendance;

public class AttendanceForUpdateDtoValidator : AbstractValidator<AttendanceForUpdateDto>
{
    public AttendanceForUpdateDtoValidator()
    {
        RuleFor(x => x.Date)
            .NotNull().WithMessage("Date is required.")
            .Must(date => date == null || date.Value <= DateTime.UtcNow)
            .WithMessage("Date cannot be in the future.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .MaximumLength(20).WithMessage("Status must not exceed 20 characters.")
            .Must(s => !string.IsNullOrWhiteSpace(s?.Trim()))
            .WithMessage("Status cannot be whitespace.")
            .Must(s =>
            {
                var allowed = new[] { "Present", "Absent", "Late" };
                return s == null || allowed.Contains(s.Trim(), StringComparer.OrdinalIgnoreCase);
            }).WithMessage("Status must be one of: Present, Absent, Late.");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("StudentId is required.");
    }
}
