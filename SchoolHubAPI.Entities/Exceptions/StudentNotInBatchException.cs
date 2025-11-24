namespace SchoolHubAPI.Entities.Exceptions;

public sealed class StudentNotInBatchException : BadRequestException
{
    public StudentNotInBatchException(Guid studentId, Guid batchId)
        : base($"Student with id: {studentId} not enrolled with bath : {batchId}.")
    {
    }
}
