using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Presentation.ActionFilters;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Submission;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/assignments/{assignmentId}/submissions")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
public class SubmissionsController : ControllerBase
{
    private readonly IServiceManager _service;

    public SubmissionsController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetSubmissions(Guid assignmentId, [FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.SubmissionService.GetAllForAssignmentAsync(assignmentId, requestParameters, assignmentTrackChanges: false, subTrackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.SubmissionDtos);
    }

    [HttpGet("{id:guid}", Name = "GetSubmissionById")]
    public async Task<IActionResult> GetSubmission(Guid assignmentId, Guid id)
    {
        var submissionDto = await _service.SubmissionService.GetByIdAsync(assignmentId, id, assignmentTrackChanges: false, subTrackChanges: false);

        return Ok(submissionDto);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateSubmission(Guid assignmentId, [FromBody] SubmissionForCreationDto creationDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        var submittionDto = await _service.SubmissionService.SubmitAsync(assignmentId, userId, creationDto, assignmentTrackChanges: false, subTrackChanges: false);

        return CreatedAtRoute("GetSubmissionById", new { assignmentId, submittionDto!.Id }, submittionDto);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateSubmission(Guid assignmentId, Guid id, [FromBody] SubmissionForUpdateDto updateDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        await _service.SubmissionService.UpdateAsync(assignmentId, userId, id, updateDto, assignmentTrackChanges: false, subTrackChanges: true);

        return NoContent();
    }

    [HttpPut("{id:guid}/grade")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> GradeSubmission(Guid assignmentId, Guid id, [FromBody] GradeSubmissionForUpdateDto gradeDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        await _service.SubmissionService.GradeAsync(assignmentId, userId, id, gradeDto, assignmentTrackChanges: false, subTrackChanges: true);

        return NoContent();
    }
}
