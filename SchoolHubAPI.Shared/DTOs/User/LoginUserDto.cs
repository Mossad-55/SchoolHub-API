namespace SchoolHubAPI.Shared.DTOs.User;

public record LoginUserDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
