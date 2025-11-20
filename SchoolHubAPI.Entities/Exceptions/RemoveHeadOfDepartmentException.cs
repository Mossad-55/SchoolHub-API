namespace SchoolHubAPI.Entities.Exceptions;

public sealed class RemoveHeadOfDepartmentException : BadRequestException
{
    public RemoveHeadOfDepartmentException(Guid teacherId) 
        : base($"Can't remove this user with Id: {teacherId}. He is currently a head of department. Please remove the current user from his role.")
    {
    }
}
