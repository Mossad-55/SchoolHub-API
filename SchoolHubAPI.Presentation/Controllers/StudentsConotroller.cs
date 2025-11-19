using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.RequestFeatures;
using System.Text.Json;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/students")]
[ApiController]
public class StudentsConotroller : ControllerBase
{
    private readonly IServiceManager _service;

    public StudentsConotroller(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetStudents([FromQuery] RequestParameters requestParameters)
    {
        var result = await _service.StudentService.GetAllAsync(requestParameters, trackChanges: false);

        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.StudentDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var studentDto = await _service.StudentService.GetByIdAsync(id, trackChanges: false);

        return Ok(studentDto);
    }
}
