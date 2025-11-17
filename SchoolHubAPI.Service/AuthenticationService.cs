using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.ConfigurationModels;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SchoolHubAPI.Service;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IServiceManager _serviceManager;
    private readonly ILoggerManager _logger;
    private readonly IOptions<JwtConfiguration> _options;
    private readonly IMapper _mapper;

    private readonly JwtConfiguration _jwtConfiguration;
    private User? _user;

    public AuthenticationService(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager
        , IServiceManager serviceManager, IOptions<JwtConfiguration> options
        , IMapper mapper, ILoggerManager logger)
    {
        _serviceManager = serviceManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _logger = logger;
        _options = options;
        _jwtConfiguration = options.Value;
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new FailedToDeleteUserException(userId);
    }

    public async Task<TokenDto> GenerateRefreshTokenAsync(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken!);

        var user = await _userManager.FindByNameAsync(principal.Identity!.Name!);
        if (user is null || user.RefreshToken != tokenDto.RefreshToken
            || user.RefreshTokenExpiryDate <= DateTime.Now)
            throw new InvalidRefreshTokenException();

        _user = user;

        return await GenerateTokenAsync(populateExpiry: false);
    }

    public async Task<TokenDto> GenerateTokenAsync(bool populateExpiry)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOPtions = GenerateTokenOptions(signingCredentials, claims);

        var refreshToken = GenerateRefreshToken();

        _user!.RefreshToken = refreshToken;
        if (populateExpiry)
            _user.RefreshTokenExpiryDate = DateTime.Now.AddDays(5);

        await _userManager.UpdateAsync(_user);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOPtions);

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginDto)
    {
        _user = await _userManager.FindByEmailAsync(loginDto.Email ?? string.Empty) ?? throw new WrongEmailOrPasswordException();

        var result = await _userManager.CheckPasswordAsync(_user, loginDto.Password ?? string.Empty);
        if (!result)
            throw new WrongEmailOrPasswordException();

        var tokenDto = await GenerateTokenAsync(populateExpiry: true);
        var userRoles = await _userManager.GetRolesAsync(_user);

        var response = new LoginResponseDto
        {
            AccessToken = tokenDto.AccessToken,
            ExpiresAt = _user.RefreshTokenExpiryDate,
            RefreshToken = tokenDto.RefreshToken,
            Role = userRoles.SingleOrDefault(),
            UserId = _user.Id,
            Email = _user.Email
        };

        return response;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserRegisterationDto registerDto)
    {
        // Check if email exists
        var exisitingUser = await _userManager.FindByEmailAsync(registerDto.Email!);
        if (exisitingUser is not null)
            throw new EmailExistsException(registerDto.Email!);

        // Check if role exists
        if (!await _roleManager.RoleExistsAsync(registerDto.Role!))
            throw new RoleNotFoundException(registerDto.Role!);

        var userEntity = _mapper.Map<User>(registerDto);

        var result = await _userManager.CreateAsync(userEntity, registerDto.Password!);
        if (!result.Succeeded)
            return result;

        await _userManager.AddToRoleAsync(userEntity, registerDto.Role);

        switch (registerDto.Role)
        {
            case "Admin":
                await _serviceManager.AdminService.CreateAsync(userEntity.Id, false);
                break;
            case "Teacher":
                await _serviceManager.TeacherService.CreateAsync(userEntity.Id, false);
                break;
            case "Student":
                await _serviceManager.StudentService.CreateAsync(userEntity.Id, false);
                break;
            default:
                throw new RoleNotFoundException(registerDto.Role!);
        }

        return result;
    }

    public async Task<IdentityResult> UpdateUserAsync(Guid userId, UserUpdateDto updateDto)
    {
        var userEntity = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        // Mapping
        _mapper.Map(userEntity, updateDto);

        var result = await _userManager.UpdateAsync(userEntity);

        return result;
    }

    // Private Functions For JWT Token Generation
    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _user!.Id.ToString()),
            new Claim(ClaimTypes.Name, _user.Name!),
            new Claim(ClaimTypes.Email, _user.Email!)
        };

        var roles = await _userManager.GetRolesAsync(_user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken
        (
            issuer: _jwtConfiguration.ValidIssuer,
            audience: _jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
            signingCredentials: signingCredentials
        );

        return tokenOptions;
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        if (string.IsNullOrEmpty(_jwtConfiguration.SecretKey))
            throw new InvalidOperationException("JWT Secret Key is not configured.");

        try
        {
            var key = Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey);

            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to create signing credentials", ex);
        }
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey!)),
            ValidateLifetime = true, // We want to validate an expired token
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null)
            throw new SecurityTokenException("Invalid token format");

        if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token algorithm. Expected HMAC-SHA256");

        return principal;
    }
}
