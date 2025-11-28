namespace SchoolHubAPI.Entities.Exceptions;

public sealed class UserIsAlreadyInActiveException : BadRequestException
{
    public UserIsAlreadyInActiveException(Guid id) 
        : base($"User with id: {id} is already inactive.")
    {
    }
}
