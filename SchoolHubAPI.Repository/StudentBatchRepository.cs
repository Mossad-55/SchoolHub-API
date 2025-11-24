using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Repository;

internal sealed class StudentBatchRepository : RepositoryBase<StudentBatch>, IStudentBatchRepository
{
    public StudentBatchRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public void Enroll(StudentBatch studentBatch) => Create(studentBatch);

    public async Task<bool> ExistsAsync(Guid studentId, Guid batchId, bool trackChanges) =>
        await FindByCondition(sb => sb.StudentId == studentId && sb.BatchId == batchId, trackChanges)
            .SingleOrDefaultAsync() != null;

    public async Task<StudentBatch?> GetByIdAsync(Guid id, bool trackChanges) =>
        await FindByCondition(sb => sb.Id == id, trackChanges)
            .Include(sb => sb.Batch)
            .Include(sb => sb.Student!.User)
            .SingleOrDefaultAsync();

    public async Task<StudentBatch?> GetByIdForBatchAsync(Guid batchId, Guid id, bool trackChanges) =>
        await FindByCondition(sb => sb.BatchId == batchId && sb.Id == id, trackChanges)
            .Include(sb => sb.Batch)
            .Include(sb => sb.Student!.User)
            .SingleOrDefaultAsync();

    public async Task<PagedList<StudentBatch>> GetStudentBatchesAsync(Guid batchId, RequestParameters requestParameters, bool trackChanges)
    {
        var studentBatches = await FindByCondition(sb => sb.BatchId == batchId, trackChanges)
            .Sort(requestParameters.OrderBy!)
            .Include(sb => sb.Batch)
            .Include(sb => sb.Student!.User)
            .ToListAsync();

        return PagedList<StudentBatch>.ToPagedList(studentBatches, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public void Remove(StudentBatch studentBatch) => Delete(studentBatch);
}
