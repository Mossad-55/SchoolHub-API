using Microsoft.AspNetCore.Authorization;
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
[Authorize(Roles = "Teacher, Student")]
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
    [Authorize(Roles = "Teacher")]
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
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateSubmission(Guid assignmentId, [FromForm] SubmissionForCreationDto creationDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return BadRequest(new { message = "Invalid user identifier." });

        string savedFilePath = string.Empty;

        try
        {
            if (!await _service.SubmissionService.CheckForSubmissionAsync(assignmentId, userId, false, false))
            {
                savedFilePath = await _fileService.SaveFileAsync(creationDto.File!);
                creationDto.FilePath = savedFilePath;
            }

            var submissionDto = await _service.SubmissionService.SubmitAsync(
                assignmentId, userId, creationDto, false, false);

            return CreatedAtRoute("GetSubmissionById", new { assignmentId, submissionDto!.Id }, submissionDto);
        }
        catch (Exception)
        {
            if (!string.IsNullOrWhiteSpace(savedFilePath))
                _fileService.DeleteFile(savedFilePath);

            throw;
        }
    }


    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> UpdateSubmission(Guid assignmentId, Guid id, [FromForm] SubmissionForUpdateDto updateDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return BadRequest(new { message = "Invalid user identifier." });

        string newFilePath = string.Empty;
        string oldFilePath = updateDto.FilePath ?? string.Empty;

        try
        {
            if (updateDto.File != null)
            {
                newFilePath = await _fileService.SaveFileAsync(updateDto.File);
                updateDto.FilePath = newFilePath;
            }
            else if (string.IsNullOrWhiteSpace(updateDto.FilePath))
            {
                updateDto.FilePath = string.Empty;
            }

            await _service.SubmissionService.UpdateAsync(
                assignmentId, userId, id, updateDto, false, true);

            // Only delete old file if update succeeded and user uploaded a new one
            if (updateDto.File != null && !string.IsNullOrWhiteSpace(oldFilePath))
                _fileService.DeleteFile(oldFilePath);

            return NoContent();
        }
        catch (Exception)
        {
            if (!string.IsNullOrWhiteSpace(newFilePath))
                _fileService.DeleteFile(newFilePath);

            throw;
        }
    }


    [HttpPut("{id:guid}/grade")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [Authorize(Roles = "Teacher")]
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
