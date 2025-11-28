namespace SchoolHubAPI.Entities.Exceptions;

public sealed class UserNotAcvticeException : BadRequestException
{
    public UserNotAcvticeException() 
        : base($"Your account has been deactivated. Please connect with your system administrator")
    {
    }
}
