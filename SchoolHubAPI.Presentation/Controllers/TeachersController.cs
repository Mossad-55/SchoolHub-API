using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/teachers")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(Roles = "Admin")]
public class TeachersController : ControllerBase
{
    private readonly IServiceManager _service;

    public TeachersController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetTeachers([FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.TeacherService.GetAllAsync(requestParameters, trackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.TeacherDtos);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Teacher, Admin")]
    public async Task<IActionResult> GetTeacher(Guid id)
    {
        var teacherDto = await _service.TeacherService.GetByIdAsync(id, trackChanges: false);

        return Ok(teacherDto);
    }

    [HttpGet("batches")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetBatchesForTeacher([FromQuery] RequestParameters requestParameters)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        var result = await _service.BatchService.GetAllAsyncForTeacher(userId, requestParameters, batchTrackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.BatchDtos);
    }
}
