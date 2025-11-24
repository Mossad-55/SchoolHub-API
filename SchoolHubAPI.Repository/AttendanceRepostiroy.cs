using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Repository;

internal sealed class AttendanceRepostiroy : RepositoryBase<Attendance>, IAttendanceRepository
{
    public AttendanceRepostiroy(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public void AddAttendanceAsync(Attendance attendance) => Create(attendance);

    public async Task<bool> ExistsAsync(Guid batchId, Guid studentId, bool trackChanges) =>
        await FindByCondition(a => a.StudentId == studentId && a.BatchId == batchId, trackChanges)
        .SingleOrDefaultAsync() != null;

    public async Task<PagedList<Attendance>> GetAllForBatchAsync(Guid batchId, RequestParameters requestParameters, bool trackChanges)
    {
        var attendaceEntities = await FindByCondition(a => a.BatchId == batchId, trackChanges)
            .Search(requestParameters.SearchTerm!)
            .Sort(requestParameters.OrderBy!)
            .ToListAsync();

        return PagedList<Attendance>.ToPagedList(attendaceEntities, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<PagedList<Attendance>> GetAllForStudenthAsync(Guid batchId, Guid studentId, RequestParameters requestParameters, bool trackChanges)
    {
        var attendaceEntities = await FindByCondition(a => a.BatchId == batchId && a.StudentId == studentId, trackChanges)
             .Search(requestParameters.SearchTerm!)
             .Sort(requestParameters.OrderBy!)
             .ToListAsync();

        return PagedList<Attendance>.ToPagedList(attendaceEntities, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public void RemoveAttendanceAsync(Attendance attendance) => Delete(attendance);
}
