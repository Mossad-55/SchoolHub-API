namespace SchoolHubAPI.Contracts;

public interface IRepositoryManager
{
    // Repositories for different entities
    IAdminRepository Admin { get; }
    ITeacherRepository Teacher { get; }


    // Shared Methods.
    Task SaveChangesAsync();
}
