namespace SchoolHubAPI.Shared.DTOs.Notification;

public record NotificationForCreationDto
{
    public string? Title { get; init; }
    public string? Message { get; init; }
    public RecipientRole? RecipientRole { get; init; }
    public Guid? RecipientId { get; init; }
}
