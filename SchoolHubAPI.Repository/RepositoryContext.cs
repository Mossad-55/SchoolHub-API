using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Entities.Entities;

namespace SchoolHubAPI.Repository;

public class RepositoryContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public RepositoryContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add Configuration Data and Roles.

    }

    // DbSets

}
