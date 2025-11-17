using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SchoolHubAPI.Repository.Configuration;

public class RolesConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData
        (
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("d0a1fbd9-1d3f-4e38-bf9e-aac1e60e5a77"),
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("f1a0f0e5-8423-44d7-b1b2-ccda2a876a23"),
                Name = "Teacher",
                NormalizedName = "TEACHER"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("9b36d7e3-df7a-4c2f-a7df-1b7db92900c9"),
                Name = "Student",
                NormalizedName = "STUDENT"
            }
        );
    }
}
