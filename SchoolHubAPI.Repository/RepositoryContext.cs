using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Configuration;

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

        modelBuilder.Entity<Course>()
            .HasIndex(c => new { c.DepartmentId, c.Code })
            .IsUnique();

        // Add Configuration Data and Roles.
        modelBuilder.ApplyConfiguration(new RolesConfiguration());

    }

    // DbSets
    public DbSet<Admin>? Admins { get; set; }
    public DbSet<Teacher>? Teachers { get; set; }
    public DbSet<Student>? Students { get; set; }
    public DbSet<Department>? Departments { get; set; }
    public DbSet<Course>? Courses { get; set; }
}
