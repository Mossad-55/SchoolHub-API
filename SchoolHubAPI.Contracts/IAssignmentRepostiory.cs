using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface IAssignmentRepostiory
{
    Task<PagedList<Assignment>> GetAllForBatchAsync(Guid batchId, RequestParameters requestParameters, bool trackChanges);
    Task<Assignment?> GetByIdAsync(Guid id, bool trackChanges);
    Task<Assignment?> GetForBatchByIdAsync(Guid batchId, Guid id, bool trackChanges);
    void DeleteAssignment(Assignment assignment);
    void UpdateAssignment(Assignment assignment);
    void CreateAssignment(Assignment assignment);
}
