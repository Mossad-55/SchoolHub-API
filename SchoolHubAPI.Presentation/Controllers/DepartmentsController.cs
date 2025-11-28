using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SchoolHubAPI.Presentation.ActionFilters;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Department;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/departments")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Authorize(Roles = "Admin")]
public class DepartmentsController : ControllerBase
{
    private readonly IServiceManager _service;
    private readonly IMemoryCache _memoryCache;
    private const string DepartmentCacheKey = "DepartmentsCache";

    public DepartmentsController(IServiceManager service, IMemoryCache memoryCache)
    {
        _service = service;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Teacher, Student")]
    public async Task<IActionResult> GetDepartments([FromQuery] RequestParameters requestParameters)
    {
        // Try get from cache first
        if (!_memoryCache.TryGetValue(DepartmentCacheKey, out (IEnumerable<DepartmentDto> Departments, MetaData MetaData) cachedDepartments))
        {
            // Fetch from service if not cached
            cachedDepartments = await _service.DepartmentService.GetAllAsync(requestParameters, false);

            // Cache options (example: expire after 5 minutes)
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _memoryCache.Set(DepartmentCacheKey, cachedDepartments, cacheEntryOptions);
        }

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(cachedDepartments.MetaData));
        return Ok(cachedDepartments.Departments);
    }

    [HttpGet("{id:guid}", Name = "DepartmentById")]
    [Authorize(Roles = "Admin, Teacher, Student")]
    public async Task<IActionResult> GetDepartment(Guid id)
    {
        var departmentDto = await _service.DepartmentService.GetByIdAsync(id, false);
        return Ok(departmentDto);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateDepartment([FromBody] DepartmentForCreationDto creationDto)
    {
        var createdDepartment = await _service.DepartmentService.CreateAsync(creationDto, false);

        // Invalidate cache
        _memoryCache.Remove(DepartmentCacheKey);

        return CreatedAtRoute("DepartmentById", new { createdDepartment!.Id }, createdDepartment);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] DepartmentForUpdateDto updateDto)
    {
        await _service.DepartmentService.UpdateAsync(id, updateDto, true);

        // Invalidate cache
        _memoryCache.Remove(DepartmentCacheKey);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDepartment(Guid id)
    {
        await _service.DepartmentService.DeleteAsync(id, false);

        // Invalidate cache
        _memoryCache.Remove(DepartmentCacheKey);

        return NoContent();
    }
}
