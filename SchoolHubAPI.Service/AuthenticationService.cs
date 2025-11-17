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
        _logger.LogInfo($"Attempting to delete user {userId}");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            _logger.LogWarn($"Delete failed. User not found: {userId}");
            throw new UserNotFoundException(userId);
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogError($"Failed to delete user {userId}. Errors: {errors}");
            throw new FailedToDeleteUserException(userId);
        }

        _logger.LogInfo($"Successfully deleted user {userId}");
    }

    public async Task<TokenDto> GenerateRefreshTokenAsync(TokenDto tokenDto)
    {
        _logger.LogDebug("Generating refresh token from provided token DTO");

        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken!);

        var user = await _userManager.FindByNameAsync(principal.Identity!.Name!);
        if (user is null)
        {
            _logger.LogWarn("Refresh token generation failed: user (from token) not found");
            throw new InvalidRefreshTokenException();
        }

        if (user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryDate <= DateTime.Now)
        {
            _logger.LogWarn($"Invalid or expired refresh token for user {user.Id}");
            throw new InvalidRefreshTokenException();
        }

        _user = user;

        _logger.LogInfo($"Refresh token validated for user {_user.Id}. Generating new access token.");
        return await GenerateTokenAsync(populateExpiry: false);
    }

    public async Task<TokenDto> GenerateTokenAsync(bool populateExpiry)
    {
        _logger.LogDebug($"Generating JWT token (populateExpiry={populateExpiry}) for user {_user?.Id}");

        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOPtions = GenerateTokenOptions(signingCredentials, claims);

        var refreshToken = GenerateRefreshToken();

        _user!.RefreshToken = refreshToken;
        if (populateExpiry)
            _user.RefreshTokenExpiryDate = DateTime.Now.AddDays(5);

        var updateResult = await _userManager.UpdateAsync(_user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
            _logger.LogError($"Failed to update user {_user.Id} with refresh token. Errors: {errors}");

            throw new InvalidRefreshTokenException();
        }
        else
        {
            _logger.LogDebug($"Stored refresh token for user {_user.Id}");
        }

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOPtions);

        _logger.LogInfo($"Generated access token for user {_user.Id}");

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginDto)
    {
        _logger.LogInfo($"Login attempt for email '{loginDto.Email}'");

        _user = await _userManager.FindByEmailAsync(loginDto.Email ?? string.Empty);
        if (_user is null)
        {
            _logger.LogWarn($"Login failed: email not found '{loginDto.Email}'");
            throw new WrongEmailOrPasswordException();
        }

        var result = await _userManager.CheckPasswordAsync(_user, loginDto.Password ?? string.Empty);
        if (!result)
        {
            _logger.LogWarn($"Login failed for user {_user.Id}: wrong password");
            throw new WrongEmailOrPasswordException();
        }

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

        _logger.LogInfo($"User {_user.Id} logged in successfully with role '{response.Role}'");

        return response;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserRegisterationDto registerDto)
    {
        _logger.LogInfo($"Registering user with email '{registerDto.Email}' and role '{registerDto.Role}'");

        // Check if email exists
        var exisitingUser = await _userManager.FindByEmailAsync(registerDto.Email!);
        if (exisitingUser is not null)
        {
            _logger.LogWarn($"Registration failed. Email already exists: {registerDto.Email}");
            throw new EmailExistsException(registerDto.Email!);
        }

        // Check if role exists
        if (!await _roleManager.RoleExistsAsync(registerDto.Role!))
        {
            _logger.LogWarn($"Registration failed. Role not found: {registerDto.Role}");
            throw new RoleNotFoundException(registerDto.Role!);
        }

        var userEntity = _mapper.Map<User>(registerDto);

        var result = await _userManager.CreateAsync(userEntity, registerDto.Password!);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogError($"User creation failed for email {registerDto.Email}. Errors: {errors}");
            return result;
        }

        _logger.LogInfo($"User created {userEntity.Id}. Assigning role '{registerDto.Role}'");
        await _userManager.AddToRoleAsync(userEntity, registerDto.Role!);

        switch (registerDto.Role)
        {
            case "Admin":
                _logger.LogDebug($"Creating Admin profile for user {userEntity.Id}");
                await _serviceManager.AdminService.CreateAsync(userEntity.Id, false);
                break;
            case "Teacher":
                _logger.LogDebug($"Creating Teacher profile for user {userEntity.Id}");
                await _serviceManager.TeacherService.CreateAsync(userEntity.Id, false);
                break;
            case "Student":
                _logger.LogDebug($"Creating Student profile for user {userEntity.Id}");
                await _serviceManager.StudentService.CreateAsync(userEntity.Id, false);
                break;
            default:
                _logger.LogError($"Registration failed. Unknown role '{registerDto.Role}' for user {userEntity.Id}");
                throw new RoleNotFoundException(registerDto.Role!);
        }

        _logger.LogInfo($"Registration completed for user {userEntity.Id} with role '{registerDto.Role}'");
        return result;
    }

    public async Task<IdentityResult> UpdateUserAsync(Guid userId, UserUpdateDto updateDto)
    {
        _logger.LogInfo($"Updating user {userId}");

        var userEntity = await _userManager.FindByIdAsync(userId.ToString());
        if (userEntity is null)
        {
            _logger.LogWarn($"Update failed. User not found: {userId}");
            throw new UserNotFoundException(userId);
        }

        // Mapping
        _mapper.Map(userEntity, updateDto);

        var result = await _userManager.UpdateAsync(userEntity);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogError($"Failed to update user {userId}. Errors: {errors}");
        }
        else
        {
            _logger.LogInfo($"User {userId} updated successfully");
        }

        return result;
    }

    // Private Functions For JWT Token Generation
    private async Task<List<Claim>> GetClaims()
    {
        _logger.LogDebug($"Building claims for user {_user?.Id}");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _user!.Id.ToString()),
            new Claim(ClaimTypes.Name, _user.Name!),
            new Claim(ClaimTypes.Email, _user.Email!)
        };

        var roles = await _userManager.GetRolesAsync(_user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        _logger.LogDebug($"Built {claims.Count} claims for user {_user.Id}");
        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        _logger.LogDebug("Generating JWT token options");
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
        _logger.LogDebug("Creating signing credentials for JWT");

        if (string.IsNullOrEmpty(_jwtConfiguration.SecretKey))
        {
            _logger.LogError("JWT Secret Key is not configured.");
            throw new InvalidOperationException("JWT Secret Key is not configured.");
        }

        try
        {
            var key = Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey);

            var secret = new SymmetricSecurityKey(key);

            _logger.LogDebug("Signing credentials created");
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create signing credentials: {ex.Message}");
            throw new InvalidOperationException("Failed to create signing credentials", ex);
        }
    }

    private string GenerateRefreshToken()
    {
        _logger.LogDebug("Generating secure refresh token");

        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var token = Convert.ToBase64String(randomNumber);

        _logger.LogDebug("Refresh token generated");
        return token;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        _logger.LogDebug("Validating expired access token to extract principal");

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
        {
            _logger.LogError("Invalid token format when extracting principal");
            throw new SecurityTokenException("Invalid token format");
        }

        if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogError("Invalid token algorithm. Expected HMAC-SHA256");
            throw new SecurityTokenException("Invalid token algorithm. Expected HMAC-SHA256");
        }

        _logger.LogDebug("Principal successfully extracted from expired token");
        return principal;
    }
}
