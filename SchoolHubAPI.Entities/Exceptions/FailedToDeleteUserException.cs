namespace SchoolHubAPI.Entities.Exceptions;

public sealed class FailedToDeleteUserException : BadRequestException
{
    public FailedToDeleteUserException(Guid userId) 
        : base($"Failed to delete user with Id: {userId}. Try again later.")
    {
    }
}
