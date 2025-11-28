namespace SchoolHubAPI.Shared.DTOs.Student;

public record StudentDto
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? CreatedDate { get; set; }
    public string? UpdatedDate { get; set; }
    public bool? IsActive { get; set; }
}
