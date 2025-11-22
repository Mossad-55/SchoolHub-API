namespace SchoolHubAPI.Service.Contracts;

public interface IServiceManager
{
    IAdminService AdminService { get; }
    ITeacherService TeacherService { get; }
    IStudentService StudentService { get; }
    IDepartmentService DepartmentService { get; }
    ICourseService CourseService { get; }
    IBatchService BatchService { get; }
}
