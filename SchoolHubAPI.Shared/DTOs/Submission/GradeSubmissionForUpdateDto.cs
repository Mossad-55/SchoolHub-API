namespace SchoolHubAPI.Shared.DTOs.Submission;

public record GradeSubmissionForUpdateDto
{
    public string? Remarks { get; set; }
    public decimal Grade { get; set; }
}
