using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Service.Contracts;

namespace SchoolHubAPI.Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IAdminService> _adminService;
    private readonly Lazy<ITeacherService> _teacherService;
    private readonly Lazy<IStudentService> _studentService;
    private readonly Lazy<IDepartmentService> _departmentService;
    private readonly Lazy<ICourseService> _courseService;

    public ServiceManager(IRepositoryManager repositoryManager, UserManager<User> userManager,
        ILoggerManager logger, IMapper mapper)
    {
        _adminService = new Lazy<IAdminService>(() => new AdminService(repositoryManager, mapper, logger));
        _teacherService = new Lazy<ITeacherService>(() => new TeacherService(repositoryManager, mapper, logger));
        _studentService = new Lazy<IStudentService>(() => new StudentService(repositoryManager, mapper, logger));
        _departmentService = new Lazy<IDepartmentService>(() => new DepartmentService(repositoryManager, userManager, mapper, logger));
        _courseService = new Lazy<ICourseService>(() => new CourseService(repositoryManager, mapper, logger));
    }

    public IAdminService AdminService => _adminService.Value;
    public ITeacherService TeacherService => _teacherService.Value;
    public IStudentService StudentService => _studentService.Value;
    public IDepartmentService DepartmentService => _departmentService.Value;
    public ICourseService CourseService => _courseService.Value;
}
