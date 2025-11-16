namespace SchoolHubAPI.Entities.Exceptions;

public sealed class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid id) 
        : base($"User with Id: {id} can't be found.")
    {
    }
}
