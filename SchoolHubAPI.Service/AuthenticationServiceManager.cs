using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.ConfigurationModels;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Service.Contracts;

namespace SchoolHubAPI.Service;

public sealed class AuthenticationServiceManager : IAuthenticationServiceManager
{
    private readonly Lazy<IAuthenticationService> _authenticationService;

    public AuthenticationServiceManager(UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOptions<JwtConfiguration> config,
        IServiceManager serviceManager,
        IMapper mapper,
        ILoggerManager logger)
    {
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(userManager, roleManager, serviceManager, config, mapper, logger));
    }

    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}
