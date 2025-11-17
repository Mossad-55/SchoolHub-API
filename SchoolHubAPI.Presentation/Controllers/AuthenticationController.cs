using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Presentation.ActionFilters;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.User;
using System.Security.Claims;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationServiceManager _authService;

    public AuthenticationController(IAuthenticationServiceManager authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginDto)
    {
        var result = await _authService.AuthenticationService.LoginUserAsync(loginDto);

        return Ok(result);
    }

    [HttpPost("register")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterationDto registerDto)
    {
        var result = await _authService.AuthenticationService.RegisterUserAsync(registerDto);

        return StatusCode(201, new { message = "Registration successful. Please login to your account." });
    }

    [HttpPut("update")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto updateDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        var result = await _authService.AuthenticationService.UpdateUserAsync(userId, updateDto);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _authService.AuthenticationService.DeleteUserAsync(id);

        return NoContent();
    }
}
