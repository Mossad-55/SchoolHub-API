namespace SchoolHubAPI.Shared.DTOs.Attendance;

public record AttendanceForUpdateDto
{
    public DateTime? Date { get; set; }
    public string? Status { get; set; }
}
