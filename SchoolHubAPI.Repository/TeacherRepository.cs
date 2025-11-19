using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Repository;

internal sealed class TeacherRepository : RepositoryBase<Teacher>, ITeacherRepository
{
    public TeacherRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }

    public void CreateTeacherAsync(Teacher teacher) => Create(teacher);

    public void DeleteTeacherAsync(Teacher teacher) => Delete(teacher);

    public async Task<PagedList<Teacher>>? GetAllTeachersAsync(RequestParameters requestParameters, bool trackChanges)
    {
        var teachers = await FindAll(trackChanges)
            .Search(requestParameters.SearchTerm!)
            .Sort(requestParameters.OrderBy!)
            .Include(t => t.User)
            .ToListAsync();

        return PagedList<Teacher>.ToPagedList(teachers, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<Teacher?> GetTeacherAsync(Guid id, bool trackChanges) =>
        await FindByCondition(t => t.UserId == id, trackChanges)
        .Include(t => t.User)
        .SingleOrDefaultAsync();

    public void UpdateTeacherAsync(Teacher teacher) => Update(teacher);
}
