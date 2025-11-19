namespace SchoolHubAPI.Entities.Exceptions;

public sealed class DepartmentNotFoundException : NotFoundException
{
    public DepartmentNotFoundException(Guid id) 
        : base($"Department with Id: {id} can't be found.")
    {
    }
}
