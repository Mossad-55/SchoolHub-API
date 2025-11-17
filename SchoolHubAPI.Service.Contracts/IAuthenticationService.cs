using Microsoft.AspNetCore.Identity;
using SchoolHubAPI.Shared.DTOs.User;

namespace SchoolHubAPI.Service.Contracts;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUserAsync(UserRegisterationDto registerDto);
    Task<IdentityResult> UpdateUserAsync(UserUpdateDto updateDto);
    Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginDto);
    Task<TokenDto> GenerateTokenAsync(bool populateExpiry);
    Task<TokenDto> GenerateRefreshTokenAsync(TokenDto tokenDto);
    Task DeleteUserAsync(Guid userId);
}
