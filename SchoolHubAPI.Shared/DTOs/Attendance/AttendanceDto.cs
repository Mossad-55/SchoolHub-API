namespace SchoolHubAPI.Shared.DTOs.Attendance;

public record AttendanceDto
{
    public Guid? Id { get; set; }
    public string? Date { get; set; }
    public string? CreatedDate { get; set; }
    public string? Status { get; set; }
    public string? BatchName { get; set; }
    public string? StudentName { get; set; }
    public string? MarkeyByTeacherName { get; set; }
}
