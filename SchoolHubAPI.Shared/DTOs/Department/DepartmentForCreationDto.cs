namespace SchoolHubAPI.Shared.DTOs.Department;

public record DepartmentForCreationDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    public Guid? HeadOfDepartmentId { get; set; }
}
