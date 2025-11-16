using AutoMapper;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Service.Contracts;

namespace SchoolHubAPI.Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IAdminService> _adminService;
    private readonly Lazy<ITeacherService> _teacherService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
    {
        _adminService = new Lazy<IAdminService>(() => new AdminService(repositoryManager, mapper, logger));
        _teacherService = new Lazy<ITeacherService>(() => new TeacherService(repositoryManager, mapper, logger));
    }

    public IAdminService AdminService => _adminService.Value;
    public ITeacherService TeacherService  => _teacherService.Value;
}
