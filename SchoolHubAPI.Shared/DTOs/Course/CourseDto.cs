namespace SchoolHubAPI.Shared.DTOs.Course;

public record CourseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int Credits { get; set; }
    public string? CreatedDate { get; set; }
    public string? UpdatedDate { get; set; }
}
