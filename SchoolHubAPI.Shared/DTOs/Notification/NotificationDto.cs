namespace SchoolHubAPI.Shared.DTOs.Notification;

public record NotificationDto
{
    public Guid? Id { get; set; }
    public string? Title { get; set; } 
    public string? Message { get; set; }
    public RecipientRole RecipientRole { get; set; }
    public Guid? RecipientId { get; set; }
    public string? CreatedDate { get; set; }
    public bool IsRead { get; set; }
}
