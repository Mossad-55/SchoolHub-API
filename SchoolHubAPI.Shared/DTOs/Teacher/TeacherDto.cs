namespace SchoolHubAPI.Shared.DTOs.Teacher;

public record TeacherDto
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? CreatedDate { get; set; }
    public string? UpdatedDate { get; set; }
}
