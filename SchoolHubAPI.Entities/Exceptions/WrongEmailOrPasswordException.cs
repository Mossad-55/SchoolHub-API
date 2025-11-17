namespace SchoolHubAPI.Entities.Exceptions;

public sealed class WrongEmailOrPasswordException : BadRequestException
{
    public WrongEmailOrPasswordException() 
        : base("Invalid Email or Password. Please try again.")
    {
    }
}
