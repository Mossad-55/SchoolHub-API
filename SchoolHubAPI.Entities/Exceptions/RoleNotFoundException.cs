namespace SchoolHubAPI.Entities.Exceptions;

public sealed class RoleNotFoundException : NotFoundException
{
    public RoleNotFoundException(string role) 
        : base($"The provided role: {role} can't be found.")
    {
    }
}
