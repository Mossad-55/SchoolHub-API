namespace SchoolHubAPI.Shared.DTOs.Assignment;

public record AssignmentForUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
}
