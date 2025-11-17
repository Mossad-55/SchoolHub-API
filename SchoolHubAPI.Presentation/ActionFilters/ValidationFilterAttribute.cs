using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SchoolHubAPI.Presentation.ActionFilters;

public class ValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];

        var param = context.ActionArguments
            .SingleOrDefault(x => x.Value?.ToString()?.Contains("Dto") == true).Value;

        if(param is null)
        {
            context.Result = new BadRequestObjectResult(new
            {
                message = $"Object is null. Controller: {controller}, Action: {action}"
            });

            return;
        }

        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            context.Result = new BadRequestObjectResult(new { errors });
        }
    }
}
