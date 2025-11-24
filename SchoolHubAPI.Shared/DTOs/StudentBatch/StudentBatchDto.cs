namespace SchoolHubAPI.Shared.DTOs.StudentBatch;

public record StudentBatchDto
{
    public Guid? Id { get; set; }
    public string? EnrollmentDate { get; set; }
    public string? StudentName { get; set; }
    public string? BatchName { get; set; }
}
