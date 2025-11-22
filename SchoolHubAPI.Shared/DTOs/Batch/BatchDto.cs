namespace SchoolHubAPI.Shared.DTOs.Batch;

public record BatchDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Semester { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string? CreatedDate { get; set; }
    public string? UpdatedDate { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? TeacherId { get; set; }
}
