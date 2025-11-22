namespace SchoolHubAPI.Entities.Exceptions;

public sealed class BatchStatusException : BadRequestException
{
    public BatchStatusException(string status) 
        : base($"The batch cannot be {status} because it is already in that state.")
    {
    }
}
