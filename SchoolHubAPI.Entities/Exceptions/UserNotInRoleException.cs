namespace SchoolHubAPI.Entities.Exceptions;

public sealed class UserNotInRoleException : BadRequestException
{
    public UserNotInRoleException(Guid userId) 
        : base($"User with id: {userId} isn't in the correct Role.")
    {
    }
}
