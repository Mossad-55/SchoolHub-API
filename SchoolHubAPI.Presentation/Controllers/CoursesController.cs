using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Presentation.ActionFilters;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Course;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/departments/{departmentId}/courses")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
public class CoursesController : ControllerBase
{
    private readonly IServiceManager _service;

    public CoursesController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetCourses(Guid departmentId, [FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.CourseService.GetAllAsync(departmentId, requestParameters,depTrackChanges: false, courseTrackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.CourseDtos);
    }

    [HttpGet("{id:guid}", Name = "CourseById")]
    public async Task<IActionResult> GetCourse(Guid departmentId, Guid id)
    {
        var courseDto = await _service.CourseService.GetByIdForDepartmentAsync(departmentId, id, depTrackChanges: false, courseTrackChanges: false);

        return Ok(courseDto);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCourse(Guid departmentId, [FromBody] CourseForCreationDto creationDto)
    {
        var courseDto = await _service.CourseService.CreateAsync(departmentId, creationDto, depTrackChanges: false, courseTrackChanges: false);

        return CreatedAtRoute("CourseById", new { departmentId, id = courseDto!.Id }, courseDto);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCourse(Guid departmentId, Guid id, [FromBody] CourseForUpdateDto updateDto)
    {
        await _service.CourseService.UpdateAsync(departmentId, id, updateDto, depTrackChanges: false, courseTrackChanges: true);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCourse(Guid departmentId, Guid id)
    {
        await _service.CourseService.DeleteAsync(departmentId, id, depTrackChanges: false, courseTrackChanges: false);

        return NoContent();
    }
}
