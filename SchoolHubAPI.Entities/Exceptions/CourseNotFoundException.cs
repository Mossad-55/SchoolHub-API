namespace SchoolHubAPI.Entities.Exceptions;

public sealed class CourseNotFoundException : NotFoundException
{
    public CourseNotFoundException(Guid id) 
        : base($"Course with Id: {id} can't be found.")
    {
    }
}
