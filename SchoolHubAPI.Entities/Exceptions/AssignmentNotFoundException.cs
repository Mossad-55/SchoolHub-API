namespace SchoolHubAPI.Entities.Exceptions;

public sealed class AssignmentNotFoundException : NotFoundException
{
    public AssignmentNotFoundException(Guid id) 
        : base($"Assignment with {id} can't be found.")
    {
    }
}
