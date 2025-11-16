using AutoMapper;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Service.Contracts;

namespace SchoolHubAPI.Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IAdminService> _adminService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
    {
        _adminService = new Lazy<IAdminService>(() => new AdminService(repositoryManager, mapper, logger));
    }

    public IAdminService AdminService => _adminService.Value;
}
