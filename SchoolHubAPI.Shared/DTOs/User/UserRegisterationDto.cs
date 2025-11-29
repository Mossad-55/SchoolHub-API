namespace SchoolHubAPI.Shared.DTOs.User;

public record UserRegisterationDto
{
    // Common Props.
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string? Role { get; set; }

    // Specific props depends on the role.

}
