namespace SchoolHubAPI.Shared.DTOs.StudentBatch;

public record BatchForStudentDto
{
    public Guid? Id { get; set; }
    public string? EnrollmentDate { get; set; }
    public string? BatchName { get; set; }
    public string? CourseName { get; set; }
    public string? Semester { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public bool IsActive { get; set; }
}
