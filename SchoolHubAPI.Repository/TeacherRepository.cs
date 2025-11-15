using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;

namespace SchoolHubAPI.Repository;

internal sealed class TeacherRepository : RepositoryBase<Teacher>, ITeacherRepository
{
    public TeacherRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }

    public void CreateTeacherAsync(Teacher teacher) => Create(teacher);

    public void DeleteTeacherAsync(Teacher teacher) => Delete(teacher);

    public async Task<List<Teacher>>? GetAllTeachersAsync(bool trackChanges) =>
        await FindAll(trackChanges)
        .Include(t => t.User)
        .ToListAsync();

    public async Task<Teacher?> GetTeacherAsync(Guid id, bool trackChanges) =>
        await FindByCondition(t => t.UserId == id, trackChanges)
        .Include(t => t.User)
        .SingleOrDefaultAsync();

    public void UpdateTeacherAsync(Teacher teacher) => Update(teacher);
}
