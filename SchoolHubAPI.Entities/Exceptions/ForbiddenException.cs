namespace SchoolHubAPI.Entities.Exceptions;

public abstract class ForbiddenException : Exception
{
    protected ForbiddenException(string message)
        : base(message)
    {
    }
}
