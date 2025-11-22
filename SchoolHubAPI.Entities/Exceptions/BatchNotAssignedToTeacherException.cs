using SchoolHubAPI.Entities.Entities;

namespace SchoolHubAPI.Entities.Exceptions;

public sealed class BatchNotAssignedToTeacherException : BadRequestException
{
    public BatchNotAssignedToTeacherException(Guid batchId) 
        : base($"Batch Id: {batchId} is not assigned to the current teacher.")
    {
    }
}
