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
    private readonly Lazy<IBatchRepsitory> _batchRepsitory;
    private readonly Lazy<IStudentBatchRepository> _studentBatchRepository;
    private readonly Lazy<IAttendanceRepository> _attendanceRepository;
    private readonly Lazy<IAssignmentRepostiory> _assignmentRepostiory;
    private readonly Lazy<ISubmissionRepository> _submissionRepository;
    
    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _adminRepository = new Lazy<IAdminRepository>(() => new AdminRepository(_repositoryContext));
        _teacherRepository = new Lazy<ITeacherRepository>(() => new TeacherRepository(_repositoryContext));
        _studentRepository = new Lazy<IStudentRepository>(() => new StudentRepository(_repositoryContext));
        _departmentRepository = new Lazy<IDepartmentRepository>(() => new DepartmentRepository(_repositoryContext));
        _courseRepository = new Lazy<ICourseRepository>(() => new CourseRepository(_repositoryContext));
        _batchRepsitory = new Lazy<IBatchRepsitory>(() => new BatchRepository(_repositoryContext));
        _studentBatchRepository = new Lazy<IStudentBatchRepository>(() => new StudentBatchRepository(_repositoryContext));
        _attendanceRepository = new Lazy<IAttendanceRepository>(() => new AttendanceRepostiroy(_repositoryContext));
        _assignmentRepostiory = new Lazy<IAssignmentRepostiory>(() => new AssignmentRepository(_repositoryContext));
        _submissionRepository = new Lazy<ISubmissionRepository>(() => new SubmissionsRepository(_repositoryContext));
    }

    // Repositories 
    public IAdminRepository Admin => _adminRepository.Value;
    public ITeacherRepository Teacher => _teacherRepository.Value;
    public IStudentRepository Student => _studentRepository.Value;
    public IDepartmentRepository Department => _departmentRepository.Value;
    public ICourseRepository Course => _courseRepository.Value;
    public IBatchRepsitory Batch => _batchRepsitory.Value;
    public IStudentBatchRepository StudentBatch => _studentBatchRepository.Value;
    public IAttendanceRepository Attendance => _attendanceRepository.Value;
    public IAssignmentRepostiory Assignment => _assignmentRepostiory.Value;
    public ISubmissionRepository Submission => _submissionRepository.Value;
    
    // Common Methods
    public async Task SaveChangesAsync() => await _repositoryContext.SaveChangesAsync();
}
