using SchoolHubAPI.Contracts;

namespace SchoolHubAPI.Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IAdminRepository> _adminRepository;
    private readonly Lazy<ITeacherRepository> _teacherRepository;
    private readonly Lazy<IStudentRepository> _studentRepository;
    private readonly Lazy<IDepartmentRepository> _departmentRepository;
    private readonly Lazy<ICourseRepository> _courseRepository;
    
    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _adminRepository = new Lazy<IAdminRepository>(() => new AdminRepository(_repositoryContext));
        _teacherRepository = new Lazy<ITeacherRepository>(() => new TeacherRepository(_repositoryContext));
        _studentRepository = new Lazy<IStudentRepository>(() => new StudentRepository(_repositoryContext));
        _departmentRepository = new Lazy<IDepartmentRepository>(() => new DepartmentRepository(_repositoryContext));
        _courseRepository = new Lazy<ICourseRepository>(() => new CourseRepository(_repositoryContext));
    }

    // Repositories 
    public IAdminRepository Admin => _adminRepository.Value;
    public ITeacherRepository Teacher => _teacherRepository.Value;
    public IStudentRepository Student => _studentRepository.Value;
    public IDepartmentRepository Department => _departmentRepository.Value;
    public ICourseRepository Course => _courseRepository.Value;
    
    // Common Methods
    public async Task SaveChangesAsync() => await _repositoryContext.SaveChangesAsync();
}
