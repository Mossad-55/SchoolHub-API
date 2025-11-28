using Microsoft.AspNetCore.Http;

namespace SchoolHubAPI.Shared.DTOs.Submission;

public record SubmissionForUpdateDto
{
    public DateTime? SubmittedDate { get; set; } = DateTime.UtcNow;
    public IFormFile? File { get; set; }
    public string? FilePath { get; set; }
}
