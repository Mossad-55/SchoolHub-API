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
            .Include(a => a.Batch)
            .Include(a => a.Teacher).ThenInclude(t => t!.User)
            .Include(a => a.Student).ThenInclude(s => s!.User)
            .ToListAsync();

        return PagedList<Attendance>.ToPagedList(attendaceEntities, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<Attendance?> GetAttendanceForStudenthAsync(Guid batchId, Guid studentId, bool trackChanges) =>
        await FindByCondition(a => a.BatchId == batchId && a.StudentId == studentId, trackChanges)
             .Include(a => a.Batch)
             .Include(a => a.Teacher).ThenInclude(t => t!.User)
             .Include(a => a.Student).ThenInclude(s => s!.User)
             .SingleOrDefaultAsync();
        

    public async Task<Attendance?> GetAttendanceForBatch(Guid batchId, Guid attendanceId, bool trackChanges) =>
        await FindByCondition(a => a.Id == attendanceId && a.BatchId == batchId, trackChanges)
            .Include(a => a.Batch)
            .Include(a => a.Teacher).ThenInclude(t => t!.User)
            .Include(a => a.Student).ThenInclude(s => s!.User)
            .SingleOrDefaultAsync();

    public void RemoveAttendanceAsync(Attendance attendance) => Delete(attendance);
}
