using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Notification;

namespace SchoolHubAPI.Shared.Validators.Notification;

public class NotificationForUpdateDtoValidator : AbstractValidator<NotificationForUpdateDto>
{
    public NotificationForUpdateDtoValidator()
    {
        RuleFor(n => n.Title)
               .NotEmpty().WithMessage("Notification title is required.")
               .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(n => n.Message)
            .NotEmpty().WithMessage("Notification message is required.")
            .MaximumLength(1000).WithMessage("Message cannot exceed 1000 characters.");
    }
}
