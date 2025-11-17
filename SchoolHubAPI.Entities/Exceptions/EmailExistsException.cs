namespace SchoolHubAPI.Entities.Exceptions;

public sealed class EmailExistsException : BadRequestException
{
    public EmailExistsException(string email) 
        : base($"The provided email: {email} has been registered before.")
    {
    }
}
