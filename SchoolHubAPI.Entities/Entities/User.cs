using Microsoft.AspNetCore.Identity;

namespace SchoolHubAPI.Entities.Entities;

public class User : IdentityUser<Guid>  
{
    public string? Name { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Roles Profiles
    public virtual Admin? Admin { get; set; }
    public virtual Student? Student { get; set; }
    public virtual Teacher? Teacher { get; set; }
}
