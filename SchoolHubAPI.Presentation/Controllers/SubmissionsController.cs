using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Contracts;
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
    private readonly IFileService _fileService;

    public SubmissionsController(IServiceManager service, IFileService fileService)
    {
        _service = service;
        _fileService = fileService;
    }

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

        if(!await _service.SubmissionService.CheckForSubmissionAsync(assignmentId, userId, false, false))
        {
            try
            {
                creationDto.FilePath = await _fileService.SaveFileAsync(creationDto.File!);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        var submittionDto = await _service.SubmissionService.SubmitAsync(assignmentId, userId, creationDto, assignmentTrackChanges: false, subTrackChanges: false);
        if(submittionDto is null)
            _fileService.DeleteFile(creationDto.FilePath!);

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

        if(updateDto.File != null)
        {
            try
            {
                if (updateDto.FilePath != null)
                    _fileService.DeleteFile(updateDto.FilePath);

                updateDto.FilePath = await _fileService.SaveFileAsync(updateDto.File);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
        }else if(updateDto.File == null && string.IsNullOrWhiteSpace(updateDto.FilePath))
        {
            _fileService.DeleteFile(updateDto.FilePath ?? string.Empty);
            updateDto.FilePath = string.Empty;
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
