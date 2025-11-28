using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Presentation.ActionFilters;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Batch;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/courses/{courseId}/batches")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(Roles = "Teacher")]
public class BatchesController : ControllerBase
{
    private readonly IServiceManager _service;

    public BatchesController(IServiceManager service) => _service = service;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetBatches(Guid courseId, [FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.BatchService.GetAllAsync(courseId, requestParameters, courseTrackChanges: false, batchTrackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.BatchDtos);
    }

    [HttpGet("{id:guid}", Name = "BatchById")]
    [Authorize(Roles = "Admin, Teacher, Student")]
    public async Task<IActionResult> GetBatch(Guid courseId, Guid id)
    {
        var batchDto = await _service.BatchService.GetByIdForCourseAsync(courseId, id, courseTrackChanges: false, batchTrackChanges: false);

        return Ok(batchDto);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateBatch(Guid courseId, [FromBody] BatchForCreationDto creationDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        var batchDto = await _service.BatchService.CreateAsync(courseId, userId, creationDto, courseTrackChanges: false, batchTrackChanges: false);

        return CreatedAtRoute("BatchById", new { courseId, batchDto!.Id }, batchDto);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateBatch(Guid courseId, Guid id, [FromBody] BatchForUpdateDto updateDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        await _service.BatchService.UpdateAsync(courseId, userId, id, updateDto, courseTrackChanges: false, batchTrackChanges: true);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBatch(Guid courseId, Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        await _service.BatchService.DeleteAsync(courseId, userId, id, courseTrackChanges: false, batchTrackChanges: false);

        return NoContent();
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> ActivateBatch(Guid courseId, Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        await _service.BatchService.ActivateAsync(courseId, userId, id, courseTrackChanges: false, batchTrackChanges: true);

        return NoContent();
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> DeActivateBatch(Guid courseId, Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        await _service.BatchService.DeActivateAsync(courseId, userId, id, courseTrackChanges: false, batchTrackChanges: true);

        return NoContent();
    }
}
