using Microsoft.AspNetCore.Mvc;
using NLog;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Extensions;
using SchoolHubAPI.FilesHandling;
using SchoolHubAPI.Presentation.ActionFilters;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Logging.ClearProviders();
builder.Services.ConfigureCors();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJwtConfigurationClass(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureAuthenticationManager();
builder.Services.ConfigureValidators();
builder.Services.ConfigureSwagger();

builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true; // Accept header support
    config.ReturnHttpNotAcceptable = true; // Return 406 if not acceptable
}).AddApplicationPart(typeof(SchoolHubAPI.Presentation.AssemblyReference).Assembly);

builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


builder.Services.AddMemoryCache();
var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "School Hub API v1");
        c.RoutePrefix = "swagger";
    });
}


if (app.Environment.IsProduction())
    app.UseHsts();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();

app.Run();
