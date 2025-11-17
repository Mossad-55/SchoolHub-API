namespace SchoolHubAPI.Shared.DTOs.User;

public record LoginResponseDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid? UserId { get; set; }
    public string? Role { get; set; }
    public string? Email { get; set; }
}
