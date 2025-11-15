using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Repository;

namespace SchoolHubAPI.Extensions;

public static class ServiceExtension
{
    // Add services to the container.
    
    // CORS Configuration
    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("X-Pagination"));
        });

    // Sql Connection Configuration
    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));


}
