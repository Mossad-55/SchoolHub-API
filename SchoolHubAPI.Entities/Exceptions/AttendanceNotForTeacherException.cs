namespace SchoolHubAPI.Entities.Exceptions;

public sealed class AttendanceNotForTeacherException : BadRequestException
{
    public AttendanceNotForTeacherException(Guid teacherId) 
        : base($"The attendance is not marked by teacher with id: {teacherId}.")
    {
    }
}
