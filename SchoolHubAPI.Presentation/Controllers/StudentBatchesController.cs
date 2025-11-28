using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/batches/{batchId}/students")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(Roles = "Teacher")]
public class StudentBatchesController : ControllerBase
{
    private readonly IServiceManager _service;

    public StudentBatchesController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetBatchEnrollments(Guid batchId, [FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.StudentBatchService.GetAllAsync(batchId, requestParameters, batchTrackChanges: false, sbTrackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.StudentBatchDtos);
    }

    [HttpPost("{studentId:guid}")]
    public async Task<IActionResult> EnrollStudent(Guid batchId, Guid studentId)
    {
        await _service.StudentBatchService.EnrollAsync(batchId, studentId, batchTrackChanges: false, sbTrackChanges: false);

        return StatusCode(201, new { message = "Student has been enrolled in the batch successfully." });
    }

    [HttpDelete("{studentId:guid}")]
    public async Task<IActionResult> RemoveStudent(Guid batchId, Guid studentId)
    {
        await _service.StudentBatchService.RemoveAsync(batchId, studentId, batchTrackChanges: false, sbTrackChanges: false);

        return NoContent();
    }
}
