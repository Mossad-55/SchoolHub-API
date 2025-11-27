using SchoolHubAPI.Shared.DTOs.Assignment;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface IAssignmentService
{
    Task<(IEnumerable<AssignmentDto> AssignmentDtos, MetaData MetaData)> GetAllForBatchAsync(Guid batchId, RequestParameters requestParameters, bool batchTrackChanges, bool assignmentTrackChanges);
    Task<AssignmentDto?> GetByIdAsync(Guid id, bool trackChanges);
    Task<AssignmentDto> GetForBatchByIdAsync(Guid batchId, Guid id, bool batchTrachChanges, bool assignmentTrackChanges);
    Task DeleteAsync(Guid batchId, Guid id, bool batchTrachChanges, bool assignmentTrackChanges);
    Task<AssignmentDto?> CreateAsync(Guid batchId, Guid teacherId, AssignmentForCreationDto creationDto, bool batchTrachChanges, bool assignmentTrackChanges);
    Task UpdateAsync(Guid batchId, Guid teacherId, Guid id, AssignmentForUpdateDto updateDto, bool batchTrachChanges, bool assignmentTrackChanges);
}
