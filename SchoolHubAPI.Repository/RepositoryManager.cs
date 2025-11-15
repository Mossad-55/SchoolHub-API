using SchoolHubAPI.Contracts;

namespace SchoolHubAPI.Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IAdminRepository> _adminRepository;
    private readonly Lazy<ITeacherRepository> _teacherRepository;
    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _adminRepository = new Lazy<IAdminRepository>(() => new AdminRepository(_repositoryContext));
        _teacherRepository = new Lazy<ITeacherRepository>(() => new TeacherRepository(_repositoryContext));
    }

    // Repositories 
    public IAdminRepository Admin => _adminRepository.Value;
    public ITeacherRepository Teacher => _teacherRepository.Value;
    
    // Common Methods
    public async Task SaveChangesAsync() => await _repositoryContext.SaveChangesAsync();
}
