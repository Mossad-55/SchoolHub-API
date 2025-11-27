namespace SchoolHubAPI.Shared.DTOs.Assignment;

public record AssignmentForCreationDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
}
