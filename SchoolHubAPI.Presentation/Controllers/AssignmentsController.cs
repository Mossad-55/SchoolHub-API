using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Presentation.ActionFilters;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Assignment;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/batchs/{batchId}/assignments")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(Roles = "Teacher")]
public class AssignmentsController : ControllerBase
{
    private readonly IServiceManager _service;

    public AssignmentsController(IServiceManager service) => _service = service;

    [HttpGet]
    [Authorize(Roles = "Teacher, Student")]
    public async Task<IActionResult> GetAllForBatch(Guid batchId, [FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.AssignmentService.GetAllForBatchAsync(batchId, requestParameters, batchTrackChanges: false, assignmentTrackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.AssignmentDtos);
    }

    [HttpGet("{id:guid}", Name = "GetAssignmentById")]
    [Authorize(Roles = "Teacher, Student")]
    public async Task<IActionResult> GetByIdForBatch(Guid batchId, Guid id)
    {
        var assignmentDto = await _service.AssignmentService.GetForBatchByIdAsync(batchId, id, batchTrachChanges: false, assignmentTrackChanges: false);

        return Ok(assignmentDto);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateAssignment(Guid batchId, [FromBody] AssignmentForCreationDto creationDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        var assignmentDto = await _service.AssignmentService.CreateAsync(batchId, userId, creationDto, batchTrachChanges: false, assignmentTrackChanges: false);

        return CreatedAtRoute("GetAssignmentById", new { batchId, assignmentDto!.Id }, assignmentDto);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateAssignment(Guid batchId, Guid id, [FromBody] AssignmentForUpdateDto updateDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        await _service.AssignmentService.UpdateAsync(batchId, userId, id, updateDto, batchTrachChanges: false, assignmentTrackChanges: true);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAssignment(Guid batchId, Guid id)
    {
        await _service.AssignmentService.DeleteAsync(batchId, id, batchTrachChanges: false, assignmentTrackChanges: false);

        return NoContent();
    }
}
