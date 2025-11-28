using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SchoolHubAPI.Presentation.ActionFilters;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Course;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/departments/{departmentId}/courses")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(Roles = "Admin")]
public class CoursesController : ControllerBase
{
    private readonly IServiceManager _service;
    private readonly IMemoryCache _memoryCache;

    public CoursesController(IServiceManager service, IMemoryCache memoryCache)
    {
        _service = service;
        _memoryCache = memoryCache;
    }

    private string GetCacheKey(Guid departmentId) => $"Courses_{departmentId}";

    [HttpGet]
    [Authorize(Roles = "Admin, Teacher, Student")]
    public async Task<IActionResult> GetCourses(Guid departmentId, [FromQuery] RequestParameters requestParameters)
    {
        var cacheKey = GetCacheKey(departmentId);

        if (!_memoryCache.TryGetValue(cacheKey, out (IEnumerable<CourseDto> Courses, MetaData MetaData) cachedCourses))
        {
            cachedCourses = await _service.CourseService.GetAllAsync(departmentId, requestParameters, depTrackChanges: false, courseTrackChanges: false);

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _memoryCache.Set(cacheKey, cachedCourses, cacheOptions);
        }

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(cachedCourses.MetaData));

        return Ok(cachedCourses.Courses);
    }

    [HttpGet("{id:guid}", Name = "CourseById")]
    [Authorize(Roles = "Admin, Teacher, Student")]
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

        // Invalidate cache for this department
        _memoryCache.Remove(GetCacheKey(departmentId));

        return CreatedAtRoute("CourseById", new { departmentId, id = courseDto!.Id }, courseDto);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCourse(Guid departmentId, Guid id, [FromBody] CourseForUpdateDto updateDto)
    {
        await _service.CourseService.UpdateAsync(departmentId, id, updateDto, depTrackChanges: false, courseTrackChanges: true);

        // Invalidate cache for this department
        _memoryCache.Remove(GetCacheKey(departmentId));

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCourse(Guid departmentId, Guid id)
    {
        await _service.CourseService.DeleteAsync(departmentId, id, depTrackChanges: false, courseTrackChanges: false);

        // Invalidate cache for this department
        _memoryCache.Remove(GetCacheKey(departmentId));

        return NoContent();
    }
}
