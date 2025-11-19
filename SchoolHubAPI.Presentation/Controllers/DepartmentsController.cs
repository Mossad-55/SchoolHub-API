using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Presentation.ActionFilters;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Department;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/departments")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    private readonly IServiceManager _service;

    public DepartmentsController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetDepartments([FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.DepartmentService.GetAllAsync(requestParameters, false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.departmentDtos);
    }

    [HttpGet("{id:guid}", Name = "DepartmentById")]
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

        return CreatedAtRoute("DepartmentById", new { createdDepartment!.Id }, createdDepartment);
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] DepartmentForUpdateDto updateDto)
    {
        await _service.DepartmentService.UpdateAsync(id, updateDto, true);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDepartment(Guid id)
    {
        await _service.DepartmentService.DeleteAsync(id, false);

        return NoContent();
    }
}
