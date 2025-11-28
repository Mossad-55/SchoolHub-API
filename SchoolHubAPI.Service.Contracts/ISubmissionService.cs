using SchoolHubAPI.Shared.DTOs.Submission;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface ISubmissionService
{
    Task<(IEnumerable<SubmissionDto> SubmissionDtos, MetaData MetaData)> GetAllForAssignmentAsync(Guid assignmentId, RequestParameters requestParameters, bool assignmentTrackChanges, bool subTrackChanges);
    Task<SubmissionDto?> GetByIdAsync(Guid assignmentId, Guid id, bool assignmentTrackChanges, bool subTrackChanges);
    Task<SubmissionDto?> SubmitAsync(Guid assignmentId, Guid studentId, SubmissionForCreationDto creationDto, bool assignmentTrackChanges, bool subTrackChanges);
    Task<bool> CheckForSubmissionAsync(Guid assignmentId, Guid studentId, bool assignmentTrackChanges, bool subTrackChanges);
    Task UpdateAsync(Guid assignmentId, Guid studentId, Guid id, SubmissionForUpdateDto updateDto, bool assignmentTrackChanges, bool subTrackChanges);
    Task GradeAsync(Guid assignmentId, Guid teacherId, Guid id, GradeSubmissionForUpdateDto gradeDto, bool assignmentTrackChanges, bool subTrackChanges);
    Task DeleteAsync(Guid assignmentId, Guid studentId, Guid id, bool assignmentTrackChanges, bool subTrackChanges);
}
