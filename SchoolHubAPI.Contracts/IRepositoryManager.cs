namespace SchoolHubAPI.Contracts;

public interface IRepositoryManager
{
    // Repositories for different entities
    IAdminRepository Admin { get; }
    ITeacherRepository Teacher { get; }
    IStudentRepository Student { get; }
    IDepartmentRepository Department { get; }

    // Shared Methods.
    Task SaveChangesAsync();
}
