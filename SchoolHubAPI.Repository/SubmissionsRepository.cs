using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Repository;

internal sealed class SubmissionsRepository : RepositoryBase<Submission>, ISubmissionRepository
{
    public SubmissionsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<bool> CheckForSubmissionAsync(Guid assignmentId, Guid studentId, bool trackChanges) =>
        await FindByCondition(s => s.AssignmentId == assignmentId && s.StudentId == studentId, trackChanges)
            .SingleOrDefaultAsync() != null;

    public void CreateSubmission(Submission assignment) => Create(assignment);

    public void DeleteSubmission(Submission assignment) => Delete(assignment);

    public async Task<PagedList<Submission>> GetAllForAssignmentAsync(Guid assignmentId, RequestParameters requestParameters, bool trackChanges)
    {
        var submissionEntities = await FindByCondition(s => s.AssignmentId == assignmentId, trackChanges)
            .Sort(requestParameters.OrderBy!)
            .Include(s => s.Assignment)
            .Include(s => s.Student).ThenInclude(s => s!.User)
            .Include(s => s.Teacher).ThenInclude(s => s!.User)
            .ToListAsync();

        return PagedList<Submission>.ToPagedList(submissionEntities, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<Submission?> GetForAssignmenthByIdAsync(Guid assignmentId, Guid id, bool trackChanges) =>
        await FindByCondition(s => s.AssignmentId == assignmentId && s.Id == id, trackChanges)
            .Include(s => s.Assignment)
            .Include(s => s.Student).ThenInclude(s => s!.User)
            .Include(s => s.Teacher).ThenInclude(s => s!.User)
            .SingleOrDefaultAsync();

    public void UpdateSubmission(Submission assignment) => Update(assignment);
}
