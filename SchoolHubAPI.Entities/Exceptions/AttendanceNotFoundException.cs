namespace SchoolHubAPI.Entities.Exceptions;

public sealed class AttendanceNotFoundException : NotFoundException
{
    public AttendanceNotFoundException(Guid id)
        : base($"Attendance with {id} can't be found.")
    {
    }
}
