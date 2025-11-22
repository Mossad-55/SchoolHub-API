using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface IBatchRepsitory
{
    Task<PagedList<Batch>> GetAllBatches(Guid courseId, RequestParameters requestParameters, bool trackChanges);
    Task<Batch?> GetBatchByIdAsync(Guid id, bool trackChanges);
    Task<Batch?> GetBatchForCourseAsync(Guid courseId, Guid id, bool trackChanges);
    Task<bool> CheckIfBatchExistsAsync(Guid courseId, Guid teacherId, string name, bool trackChanges);
    void DeleteBatch(Batch batch);
    void UpdateBatch(Batch batch);
    void CreateBatch(Batch batch);
}
