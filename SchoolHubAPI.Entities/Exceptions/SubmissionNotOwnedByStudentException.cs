namespace SchoolHubAPI.Entities.Exceptions;

public sealed class SubmissionNotOwnedByStudentException : BadRequestException
{
    public SubmissionNotOwnedByStudentException(Guid id) 
        : base($"The submission does not belong to the specified student with id: {id}.")
    {
    }
}
