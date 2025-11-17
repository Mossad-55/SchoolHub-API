namespace SchoolHubAPI.Entities.Exceptions;

public sealed class InvalidRefreshTokenException : BadRequestException
{
    public InvalidRefreshTokenException() 
        : base("Invalid request. The TokenDto has some invalid credentials.")
    {
    }
}
