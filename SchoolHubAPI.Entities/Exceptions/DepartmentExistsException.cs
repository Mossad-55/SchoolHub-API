namespace SchoolHubAPI.Entities.Exceptions;

public sealed class DepartmentExistsException : BadRequestException
{
    public DepartmentExistsException(string name) 
        : base($"Department with this name: {name} does exists. Please use a different name.")
    {
    }
}
