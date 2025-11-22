namespace SchoolHubAPI.Entities.Exceptions;

public sealed class BatchExistsException : BadRequestException
{
    public BatchExistsException(string name) 
        : base($"Batch with this name: {name} does exist for a course. Please use a different name.")
    {
    }
}
