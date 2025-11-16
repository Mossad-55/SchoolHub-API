namespace SchoolHubAPI.Shared.DTOs;

public record AdminDto
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
