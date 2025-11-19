namespace SchoolHubAPI.Shared.DTOs.Department;

public record DepartmentForUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
    public Guid? HeadOfDepartmentId { get; set; }
}
