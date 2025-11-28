using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Presentation.ActionFilters;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Attendance;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/batches/{batchId}/attendances")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(Roles = "Teacher")]
public class AttendancesController : ControllerBase
{
    private readonly IServiceManager _service;

    public AttendancesController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAllAttendancesForBatch(Guid batchId, [FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.AttendanceService.GetAttendanceForBatchAsync(batchId, requestParameters, batchTrackChanges: false, attTrackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.AttendanceDtos);
    }

    [HttpGet("{id:guid}", Name = "GetAttendanceById")]
    [Authorize(Roles = "Teacher, Student")]
    public async Task<IActionResult> GetAttendanceById(Guid batchId, Guid id)
    {
        var attendanceDto = await _service.AttendanceService.GetAttendanceForBatchAsync(batchId, id, batchTrackChanges: false, attTrackChanges: false);

        return Ok(attendanceDto);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateAttendance(Guid batchId, [FromBody] AttendanceForCreationDto creationDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        var attendanceDto = await _service.AttendanceService.CreateAttendanceAsync(batchId, userId, creationDto, batchTrackChanges: false, attTrackChanges: false);

        return CreatedAtRoute("GetAttendanceById", new { batchId, attendanceDto!.Id }, attendanceDto);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateAttendance(Guid batchId, Guid id, [FromBody] AttendanceForUpdateDto updateDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        await _service.AttendanceService.UpdateAttendanceAsync(batchId, userId, id, updateDto, batchTrackChanges: false, attTrackChanges: true);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAttendance(Guid batchId, Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        await _service.AttendanceService.DeleteAttendanceAsync(batchId, userId, id, batchTrackChanges: false, attTrackChanges: false);

        return NoContent();
    }
}
