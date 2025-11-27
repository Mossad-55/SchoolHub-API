namespace SchoolHubAPI.Shared.DTOs.Submission;

public record SubmissionDto
{
    public Guid? Id { get; set; }
    public string? FileUrl { get; set; }
    public string? SubmittedDate { get; set; }
    public decimal? Grade { get; set; }
    public string? Remarks { get; set; }
    public string? StudentName { get; set; }
    public string? AssignmentTitle { get; set; }
    public string? TeacherName { get; set; }
}
