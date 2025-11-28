namespace SchoolHubAPI.Entities.Exceptions;

public sealed class CourseIsAlreadyInActiveException : BadRequestException
{
    public CourseIsAlreadyInActiveException(Guid id) 
        : base($"Course with id: {id} is already inactive.")
    {
    }
}
