using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHubAPI.Presentation.ActionFilters;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.User;
using System.Collections.Immutable;
using System.Security.Claims;

namespace SchoolHubAPI.Presentation.Controllers;

[Route("api/auth")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
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
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(", ", result.Errors.Select(x => x.Description)) });

        return StatusCode(201, new { message = "Registration successful. Please login to your account." });
    }

    [HttpPut("update")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto updateDto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return BadRequest(new { message = "Invalid user identifier." });
        }

        var result = await _authService.AuthenticationService.UpdateUserAsync(userId, updateDto);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(", ", result.Errors.Select(x => x.Description)) });

        return NoContent();
    }

    [HttpDelete("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _authService.AuthenticationService.DeleteUserAsync(id);

        return NoContent();
    }

    [HttpPost("refresh-token")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if(!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
        {
            tokenDto.AccessToken = authHeader["Bearer ".Length..].Trim();

            var tokenToReturn = await _authService.AuthenticationService.GenerateRefreshTokenAsync(tokenDto);

            return Ok(tokenToReturn);
        }

        return BadRequest(new { StatusCode = 400, Message = "No token found in Authorization header" });
    }
}
