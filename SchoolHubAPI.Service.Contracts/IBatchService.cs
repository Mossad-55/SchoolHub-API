using SchoolHubAPI.Shared.DTOs.Batch;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface IBatchService
{
    Task<(IEnumerable<BatchDto> BatchDtos, MetaData MetaData)> GetAllAsync(Guid courseId, RequestParameters requestParameters, bool courseTrackChanges, bool batchTrackChanges);
    Task<BatchDto?> GetByIdForCourseAsync(Guid courseId, Guid id, bool courseTrackChanges, bool batchTrackChanges);
    Task<BatchDto?> CreateAsync(Guid courseId, Guid teacherId, BatchForCreationDto creationDto, bool courseTrackChanges, bool batchTrackChanges);
    Task UpdateAsync(Guid courseId, Guid teacherId, Guid id, BatchForUpdateDto updateDto, bool courseTrackChanges, bool batchTrackChanges);
    Task DeleteAsync(Guid courseId, Guid teacherId, Guid id, bool courseTrackChanges, bool batchTrackChanges);
    Task ActivateAsync(Guid courseId, Guid teacherId, Guid id, bool courseTrackChanges, bool batchTrackChanges);
    Task DeActivateAsync(Guid courseId, Guid teacherId, Guid id, bool courseTrackChanges, bool batchTrackChanges);
}
