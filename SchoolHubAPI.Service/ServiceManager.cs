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
    private readonly Lazy<IBatchService> _batchService;
    private readonly Lazy<IStudentBatchService> _studentBatchService;
    private readonly Lazy<IAttendanceService> _attendanceService;
    private readonly Lazy<IAssignmentService> _assignmentService;
    private readonly Lazy<ISubmissionService> _submissionService;
    private readonly Lazy<INotificationService> _notificationService;

    public ServiceManager(IRepositoryManager repositoryManager, UserManager<User> userManager,
        ILoggerManager logger, IMapper mapper)
    {
        _adminService = new Lazy<IAdminService>(() => new AdminService(repositoryManager, mapper, logger));
        _teacherService = new Lazy<ITeacherService>(() => new TeacherService(repositoryManager, mapper, logger));
        _studentService = new Lazy<IStudentService>(() => new StudentService(repositoryManager, mapper, logger));
        _departmentService = new Lazy<IDepartmentService>(() => new DepartmentService(repositoryManager, userManager, mapper, logger));
        _courseService = new Lazy<ICourseService>(() => new CourseService(repositoryManager, mapper, logger));
        _batchService = new Lazy<IBatchService>(() => new BatchService(repositoryManager, mapper, logger));
        _studentBatchService = new Lazy<IStudentBatchService>(() => new StudentBatchService(repositoryManager, userManager, mapper, logger));
        _attendanceService = new Lazy<IAttendanceService>(() => new AttendanceService(repositoryManager, userManager, mapper, logger));
        _assignmentService = new Lazy<IAssignmentService>(() => new AssignmentService(repositoryManager, userManager, mapper, logger));
        _submissionService = new Lazy<ISubmissionService>(() => new SubmissionService(repositoryManager, userManager, mapper, logger));
        _notificationService = new Lazy<INotificationService>(() => new NotificationService(repositoryManager, mapper, logger));
    }

    public IAdminService AdminService => _adminService.Value;
    public ITeacherService TeacherService => _teacherService.Value;
    public IStudentService StudentService => _studentService.Value;
    public IDepartmentService DepartmentService => _departmentService.Value;
    public ICourseService CourseService => _courseService.Value;
    public IBatchService BatchService => _batchService.Value;
    public IStudentBatchService StudentBatchService => _studentBatchService.Value;
    public IAttendanceService AttendanceService => _attendanceService.Value;
    public IAssignmentService AssignmentService => _assignmentService.Value;
    public ISubmissionService SubmissionService => _submissionService.Value;
    public INotificationService NotificationService => _notificationService.Value;
}
