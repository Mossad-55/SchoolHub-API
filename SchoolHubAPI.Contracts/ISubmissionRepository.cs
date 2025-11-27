using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface ISubmissionRepository
{
    Task<PagedList<Submission>> GetAllForAssignmentAsync(Guid assignmentId, RequestParameters requestParameters, bool trackChanges);
    Task<Submission?> GetForAssignmenthByIdAsync(Guid assignmentId, Guid id, bool trackChanges);
    void DeleteAssignment(Submission assignment);
    void UpdateAssignment(Submission assignment);
    void CreateAssignment(Submission assignment);
}
