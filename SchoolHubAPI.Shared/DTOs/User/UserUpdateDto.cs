namespace SchoolHubAPI.Shared.DTOs.User;

public record UserUpdateDto
{
    // Common Props.
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;

    // Specific props depends on the role.

}
