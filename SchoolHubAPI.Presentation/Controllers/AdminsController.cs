using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/admins")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(Roles = "Admin")]
public class AdminsController : ControllerBase
{
    private readonly IServiceManager _service;

    public AdminsController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAdmins([FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.AdminService.GetAllAsync(requestParameters, trackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.AdminDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAdmin(Guid id)
    {
        var adminDto = await _service.AdminService.GetByIdAsync(id, trackChanges: false);

        return Ok(adminDto);
    }
}
