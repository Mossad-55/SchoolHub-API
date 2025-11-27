using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolHubAPI.Shared.DTOs.Assignment;

public record AssignmentDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? DueDate { get; set; }
    public string? CreatedDate { get; set; }
    public string? CreatedByTeacherName { get; set; }
    public string? BatchName { get; set; }
}
