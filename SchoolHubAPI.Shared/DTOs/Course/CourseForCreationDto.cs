namespace SchoolHubAPI.Shared.DTOs.Course;

public record CourseForCreationDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int Credits { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
}
