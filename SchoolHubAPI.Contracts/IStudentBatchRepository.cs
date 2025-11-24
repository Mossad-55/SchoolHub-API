using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface IStudentBatchRepository
{
    Task<PagedList<StudentBatch>> GetStudentBatchesAsync(Guid batchId, RequestParameters requestParameters, bool trackChanges);
    Task<StudentBatch?> GetByIdForBatchAsync(Guid batchId, Guid studentId, bool trackChanges);
    Task<PagedList<StudentBatch>> GetBatchesForStudentAsync(Guid studentId, RequestParameters requestParameters, bool trackChanges);
    Task<bool> ExistsAsync(Guid studentId, Guid batchId, bool trackChanges);
    void Enroll(StudentBatch studentBatch);
    void Remove(StudentBatch studentBatch);
}
