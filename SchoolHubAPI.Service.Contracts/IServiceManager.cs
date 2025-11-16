namespace SchoolHubAPI.Service.Contracts;

public interface IServiceManager
{
    IAdminService AdminService { get; }
    ITeacherService TeacherService { get; }
}
