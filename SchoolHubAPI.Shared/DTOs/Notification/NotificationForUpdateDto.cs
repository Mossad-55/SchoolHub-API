namespace SchoolHubAPI.Shared.DTOs.Notification;

public record NotificationForUpdateDto
{
    public string? Title { get; init; }
    public string? Message { get; init; }
}
