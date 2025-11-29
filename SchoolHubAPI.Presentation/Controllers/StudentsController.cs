using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/students")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(Roles = "Student")]
public class StudentsController : ControllerBase
{
    private readonly IServiceManager _service;
    
    public StudentsController(IServiceManager service) => _service = service;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStudents([FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.StudentService.GetAllAsync(requestParameters, trackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.StudentDtos);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin, Teacher, Student")]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var studentDto = await _service.StudentService.GetByIdAsync(id, trackChanges: false);

        return Ok(studentDto);
    }

    [HttpGet("batches")]
    [Authorize(Roles = "Student, Teacher")]
    public async Task<IActionResult> GetBachtesForStudent([FromQuery] RequestParameters requestParameters)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        var result = await _service.StudentBatchService.GetAllForStudentAsync(userId, requestParameters, sbTrackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.StudentBatchDtos);
    }

    [HttpGet("batches/{batchId}/attendances")]
    [Authorize(Roles = "Student, Teacher")]
    public async Task<IActionResult> GetAttendanceForStudent(Guid batchId, [FromQuery] RequestParameters requestParameters)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        var result = await _service.AttendanceService.GetAttendanceForStudentAsync(batchId, userId, batchTrackChanges: false, attTrackChanges: false);

        return Ok(result);
    }
}
