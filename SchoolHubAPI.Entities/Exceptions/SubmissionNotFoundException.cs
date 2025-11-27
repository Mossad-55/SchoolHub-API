namespace SchoolHubAPI.Entities.Exceptions;

public sealed class SubmissionNotFoundException : NotFoundException
{
    public SubmissionNotFoundException(Guid id) 
        : base($"Submission with id: {id} can't be found.")
    {
    }
}
