namespace SchoolHubAPI.Entities.Exceptions;

public sealed class BatchNotFoundException : NotFoundException
{
    public BatchNotFoundException(Guid id) 
        : base($"Batch with Id: {id} can't be found.")
    {
    }
}
