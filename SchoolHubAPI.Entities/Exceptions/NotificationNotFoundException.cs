namespace SchoolHubAPI.Entities.Exceptions;

public sealed class NotificationNotFoundException : NotFoundException
{
    public NotificationNotFoundException() 
        : base($"Notification not found or you do not have permission to access it")
    {
    }
}
