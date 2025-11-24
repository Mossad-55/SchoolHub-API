using SchoolHubAPI.Shared.DTOs.StudentBatch;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface IStudentBatchService
{
    Task<(IEnumerable<StudentBatchDto> StudentBatchDtos, MetaData MetaData)> GetAllAsync(Guid batchId, RequestParameters requestParameters, bool batchTrackChanges, bool sbTrackChanges);
    Task<(IEnumerable<BatchForStudentDto> StudentBatchDtos, MetaData MetaData)> GetAllForStudentAsync(Guid studentId, RequestParameters requestParameters, bool sbTrackChanges);
    Task<StudentBatchDto?> GetByIdForBatchAsync(Guid batchId, Guid id, bool batchTrackChanges, bool sbTrackChanges);
    Task EnrollAsync(Guid batchId, Guid studentId, bool batchTrackChanges, bool sbTrackChanges);
    Task RemoveAsync(Guid batchId, Guid studentId, bool batchTrackChanges, bool sbTrackChanges);
}
