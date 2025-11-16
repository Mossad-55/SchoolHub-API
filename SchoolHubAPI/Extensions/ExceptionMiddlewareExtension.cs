using Microsoft.AspNetCore.Diagnostics;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.ErrorModel;
using SchoolHubAPI.Entities.Exceptions;

namespace SchoolHubAPI.Extensions;

public static class ExceptionMiddlewareExtension
{
    public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager logger)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature is not null)
                {
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        BadRequestException => StatusCodes.Status400BadRequest,
                        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    // Logging the error
                    logger.LogError($"Something went wrong: {contextFeature.Error}");

                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message
                    }.ToString());
                }
            });
        });
    }
}
