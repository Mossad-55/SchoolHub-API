namespace SchoolHubAPI.Shared.DTOs.Batch;

public record BatchForUpdateDto
{
    public string? Name { get; set; }
    public string? Semester { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
}
