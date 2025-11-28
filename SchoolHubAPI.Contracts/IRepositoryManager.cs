namespace SchoolHubAPI.Contracts;

public interface IRepositoryManager
{
    // Repositories for different entities
    IAdminRepository Admin { get; }
    ITeacherRepository Teacher { get; }
    IStudentRepository Student { get; }
    IDepartmentRepository Department { get; }
    ICourseRepository Course { get; }
    IBatchRepsitory Batch { get; }
    IStudentBatchRepository StudentBatch { get; }
    IAttendanceRepository Attendance { get; }
    IAssignmentRepostiory Assignment { get; }
    ISubmissionRepository Submission { get; }
    INotificationRepository Notification { get; }

    // Shared Methods.
    Task SaveChangesAsync();
}
