namespace SchoolHubAPI.Shared.DTOs.User;

public record TokenDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
