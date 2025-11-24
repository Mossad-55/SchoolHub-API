namespace SchoolHubAPI.Entities.Exceptions;

public sealed class StudentIsEnrolledException : BadRequestException
{
    public StudentIsEnrolledException(Guid studentId) 
        : base($"Student with id: {studentId} is enrolled in the batch.")
    {
    }
}
