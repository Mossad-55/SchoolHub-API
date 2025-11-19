namespace SchoolHubAPI.Shared.DTOs.Department;

public record DepartmentDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CreatedDate { get; set; }
    public string? UpdatedDate { get; set; }
    public string? HeadOfDepartmentName { get; set; }
    public Guid? HeadOfDepartmentId { get; set; }
}
