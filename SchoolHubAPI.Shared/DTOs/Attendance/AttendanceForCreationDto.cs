namespace SchoolHubAPI.Shared.DTOs.Attendance;

public record AttendanceForCreationDto
{
    public DateTime? Date { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? Status { get; set; }
    public Guid StudentId { get; set; }
}
