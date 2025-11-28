namespace SchoolHubAPI.Shared.DTOs.Admin;

public record AdminDto
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? CreatedDate { get; set; }
    public string? UpdatedDate { get; set; }
    public bool? IsActive { get; set; }
}
