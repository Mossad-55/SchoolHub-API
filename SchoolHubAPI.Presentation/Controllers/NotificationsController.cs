using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.DTOs.Notification;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/notifications")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
public class NotificationsController : ControllerBase
{
    private readonly IServiceManager _service;

    public NotificationsController(IServiceManager service) => _service = service;

    // Private Functions
    private RecipientRole GetCurrentUserRole()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value;
        if (claim == null || !Enum.TryParse<RecipientRole>(claim, true, out var role))
            return RecipientRole.Student; // default fallback
        return role;
    }

    private Guid? GetCurrentUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
            return null;

        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] RequestParameters requestParameters)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        var result = await _service.NotificationService.GetForUserAsync(role, requestParameters, trackChanges: false, userId);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.NotificationDtos);
    }

    [HttpGet("{id:guid}", Name = "GetById")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        var notificationDto = await _service.NotificationService.GetByIdAsync(id, role, trackChanges: false, userId);

        return Ok(notificationDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NotificationForCreationDto creationDto)
    {
        var notificationDto = await _service.NotificationService.CreateAsync(creationDto);

        return CreatedAtRoute("GetById", new { id = notificationDto.Id }, notificationDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] NotificationForUpdateDto updateDto)
    {
        await _service.NotificationService.UpdateAsync(id, updateDto, trackChanges: true);

        return NoContent();
    }

    [HttpPatch("{id:guid}/mark-read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        await _service.NotificationService.MarkAsReadAsync(id, role, trackChanges: true, userId);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.NotificationService.DeleteAsync(id, trackChanges: false);

        return NoContent();
    }
}
