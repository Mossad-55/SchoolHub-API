namespace SchoolHubAPI.Entities.Exceptions;

public class StudentSubmittedAssignmentException : BadRequestException
{
    public StudentSubmittedAssignmentException(Guid assignmentId, Guid studentId)
        : base($"Student with id: {studentId} has already submitted the assignment with id: {assignmentId}.")
    {
    }
}
