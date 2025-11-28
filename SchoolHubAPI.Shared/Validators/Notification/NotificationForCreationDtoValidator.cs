using FluentValidation;
using SchoolHubAPI.Shared.DTOs.Notification;

namespace SchoolHubAPI.Shared.Validators.Notification;

public class NotificationForCreationDtoValidator : AbstractValidator<NotificationForCreationDto>
{
    public NotificationForCreationDtoValidator()
    {
        RuleFor(n => n.Title)
                .NotEmpty().WithMessage("Notification title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(n => n.Message)
            .NotEmpty().WithMessage("Notification message is required.")
            .MaximumLength(1000).WithMessage("Message cannot exceed 1000 characters.");

        RuleFor(n => n.RecipientRole)
            .IsInEnum().WithMessage("Recipient role is invalid.");

        When(n => n.RecipientId.HasValue, () =>
        {
            RuleFor(n => n.RecipientId)
                .NotEqual(Guid.Empty).WithMessage("RecipientId must be a valid GUID when provided.");
        });
    }
}
