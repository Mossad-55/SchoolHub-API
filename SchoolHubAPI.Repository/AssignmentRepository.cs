using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Repository;

internal sealed class AssignmentRepository : RepositoryBase<Assignment>, IAssignmentRepostiory
{
    public AssignmentRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<bool> CheckIfAssignmentExistsAsync(Guid batchId, bool trackChanges) =>
        await FindByCondition(a => a.BatchId == batchId, trackChanges)
            .SingleOrDefaultAsync() != null;

    public void CreateAssignment(Assignment assignment) => Create(assignment);

    public void DeleteAssignment(Assignment assignment) => Delete(assignment);

    public async Task<PagedList<Assignment>> GetAllForBatchAsync(Guid batchId, RequestParameters requestParameters, bool trackChanges)
    {
        var assignmentEntities = await FindByCondition(a => a.BatchId == batchId, trackChanges)
            .Search(requestParameters.SearchTerm!)
            .Sort(requestParameters.OrderBy!)
            .Include(a => a.Batch)
            .Include(a => a.Teacher).ThenInclude(t => t!.User)
            .ToListAsync();

        return PagedList<Assignment>.ToPagedList(assignmentEntities, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<Assignment?> GetByIdAsync(Guid id, bool trackChanges) =>
        await FindByCondition(a => a.Id == id, trackChanges)
            .Include(a => a.Batch)
            .Include(a => a.Teacher).ThenInclude(t => t!.User)
            .SingleOrDefaultAsync();

    public async Task<Assignment?> GetForBatchByIdAsync(Guid batchId, Guid id, bool trackChanges) =>
        await FindByCondition(a => a.Id == id && a.BatchId == batchId, trackChanges)
            .Include(a => a.Batch)
            .Include(a => a.Teacher).ThenInclude(t => t!.User)
            .SingleOrDefaultAsync();

    public void UpdateAssignment(Assignment assignment) => Update(assignment);
}
