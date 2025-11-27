namespace SchoolHubAPI.Shared.DTOs.Submission;

public record SubmissionForUpdateDto
{
    public DateTime? SubmittedDate { get; set; } = DateTime.UtcNow;
    public string? FileUrl { get; set; }
}
