using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/teachers")]
[ApiController]
public class TeachersController : ControllerBase
{
    private readonly IServiceManager _service;

    public TeachersController(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetTeachers([FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.TeacherService.GetAllAsync(requestParameters, trackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.TeacherDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTeacher(Guid id)
    {
        var teacherDto = await _service.TeacherService.GetByIdAsync(id, trackChanges: false);

        return Ok(teacherDto);
    }
}
