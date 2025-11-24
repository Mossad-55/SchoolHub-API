using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface IAttendanceRepository
{
    Task<PagedList<Attendance>> GetAllForBatchAsync(Guid batchId, RequestParameters requestParameters, bool trackChanges);
    Task<PagedList<Attendance>> GetAllForStudenthAsync(Guid batchId, Guid studentId, RequestParameters requestParameters, bool trackChanges);
    Task<bool> ExistsAsync(Guid batchId, Guid studentId, bool trackChanges);
    void AddAttendanceAsync(Attendance attendance);
    void RemoveAttendanceAsync(Attendance attendance);
}
