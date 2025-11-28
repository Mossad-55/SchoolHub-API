using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.ConfigurationModels;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.LoggerService;
using SchoolHubAPI.Repository;
using SchoolHubAPI.Service;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.Validators.Assignment;
using SchoolHubAPI.Shared.Validators.Attendance;
using SchoolHubAPI.Shared.Validators.Batch;
using SchoolHubAPI.Shared.Validators.Courses;
using SchoolHubAPI.Shared.Validators.Departments;
using SchoolHubAPI.Shared.Validators.Notification;
using SchoolHubAPI.Shared.Validators.Submission;
using SchoolHubAPI.Shared.Validators.User;
using System.Text;

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

    // Identity Configuration
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentity<User, IdentityRole<Guid>>(opt =>
        {
            opt.Password.RequireDigit = true;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireUppercase = true;
            opt.Password.RequireNonAlphanumeric = true;
            opt.Password.RequiredLength = 8;
            opt.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
    }

    // JWT Configuration
    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = new JwtConfiguration();
        configuration.Bind(jwtConfiguration.Section, jwtConfiguration);
        // Small Note: I used the secret key directly in the JWT configuration here for simplicity.

        services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtConfiguration.ValidIssuer,
                ValidAudience = jwtConfiguration.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey!)),

                ClockSkew = TimeSpan.Zero
            };
        });
    }

    // JwtConfiguration class Binding
    public static void AddJwtConfigurationClass(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));

    // Repository Manager Configuration
    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    // Logger Configuration
    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    // Service Manager Configuration
    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();

    // Authentication Service Manager Configuration
    public static void ConfigureAuthenticationManager(this IServiceCollection services) =>
        services.AddScoped<IAuthenticationServiceManager, AuthenticationServiceManager>();

    // Swagger Configuration
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "School Hub API",
                Version = "v1",
                Description = "This is backend API using .NET Core Web API to manage students, teachers, courses, classes, attendance, and grading with role-based access.",
                Contact = new OpenApiContact
                {
                    Name = "Mossad Ahmed",
                    Email = "mosad55522@gmail.com",
                    Url = new Uri("https://www.linkedin.com/in/moss-ad-ahmed-28aaa6203/")
                }
            });

            var xmlFile = $"{typeof(Presentation.AssemblyReference).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            // Only include XML comments if the file exists
            if (File.Exists(xmlPath))
            {
                s.IncludeXmlComments(xmlPath);
            }

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "JWT place holder",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }                    
                    },
                    new string[] {}
                }
            });
        });
    }

    // Validators Configuration
    public static void ConfigureValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();

        // Register all the validators
        services.AddValidatorsFromAssemblyContaining<LoginUserDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<UserUpdateDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<UserRegisterationDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<TokenDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<DepartmentForCreationDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<DepartmentForUpdateDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<CourseForCreationDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<CourseForUpdateDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<BatchForUpdateDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<BatchForCreationDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<AttendanceForCreationDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<AttendanceForUpdateDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<AssignmentForCreationDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<AssignmentForUpdateDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<SubmissionForCreationDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<SubmissionForUpdateDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<GradeSubmissionForUpdateDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<NotificationForCreationDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<NotificationForUpdateDtoValidator>();
    }
}
