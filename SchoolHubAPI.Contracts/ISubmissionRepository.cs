using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface ISubmissionRepository
{
    Task<PagedList<Submission>> GetAllForAssignmentAsync(Guid assignmentId, RequestParameters requestParameters, bool trackChanges);
    Task<Submission?> GetForAssignmenthByIdAsync(Guid assignmentId, Guid id, bool trackChanges);
    Task<bool> CheckForSubmissionAsync(Guid assignmentId, Guid studentId, bool trackChanges);
    void DeleteSubmission(Submission assignment);
    void UpdateSubmission(Submission assignment);
    void CreateSubmission(Submission assignment);
}
