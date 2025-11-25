using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Repository;

internal sealed class BatchRepository : RepositoryBase<Batch>, IBatchRepsitory
{
    public BatchRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<bool> CheckIfBatchExistsAsync(Guid courseId, Guid teacherId, string name, bool trackChanges) =>
        await FindByCondition(b => b.CourseId == courseId && b.TeacherId == teacherId
            && b.Name == name, trackChanges)
        .SingleOrDefaultAsync() != null;

    public void CreateBatch(Batch batch) => Create(batch);

    public void DeleteBatch(Batch batch) => Delete(batch);

    public async Task<PagedList<Batch>> GetAllBatchesForCourse(Guid courseId, RequestParameters requestParameters, bool trackChanges)
    {
        var batches = await FindByCondition(b => b.CourseId == courseId, trackChanges)
            .Search(requestParameters.SearchTerm!)
            .Sort(requestParameters.OrderBy!)
            .ToListAsync();

        return PagedList<Batch>.ToPagedList(batches, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<PagedList<Batch>> GetAllBatchesForTeacher(Guid teacherId, RequestParameters requestParameters, bool trackChanges)
    {
        var batches = await FindByCondition(b => b.TeacherId == teacherId, trackChanges)
           .Search(requestParameters.SearchTerm!)
           .Sort(requestParameters.OrderBy!)
           .ToListAsync();

        return PagedList<Batch>.ToPagedList(batches, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<Batch?> GetBatchByIdAsync(Guid id, bool trackChanges) =>
        await FindByCondition(b => b.Id == id, trackChanges)
            .SingleOrDefaultAsync();

    public async Task<Batch?> GetBatchForCourseAsync(Guid courseId, Guid id, bool trackChanges) =>
        await FindByCondition(b => b.CourseId == courseId && b.Id == id, trackChanges)
            .SingleOrDefaultAsync();

    public void UpdateBatch(Batch batch) => Update(batch);
}
