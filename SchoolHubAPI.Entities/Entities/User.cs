using Microsoft.AspNetCore.Identity;

namespace SchoolHubAPI.Entities.Entities;

public class User : IdentityUser<Guid>  
{
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    // Roles Profiles

}
