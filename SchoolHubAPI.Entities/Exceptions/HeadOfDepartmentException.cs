namespace SchoolHubAPI.Entities.Exceptions;

public sealed class HeadOfDepartmentException : BadRequestException
{
    public HeadOfDepartmentException() : 
        base("Head of department can be a Teacher only. Please select another User.")
    {
    }
}
