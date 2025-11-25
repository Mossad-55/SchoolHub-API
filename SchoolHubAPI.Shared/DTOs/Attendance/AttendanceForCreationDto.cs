namespace SchoolHubAPI.Shared.DTOs.Attendance;

public record AttendanceForCreationDto
{
    public DateTime? Date { get; set; } = DateTime.UtcNow;
    public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    public string? Status { get; set; }
    public Guid StudentId { get; set; }
}
