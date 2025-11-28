using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Repository;

internal sealed class StudentRepository : RepositoryBase<Student>, IStudentRepository
{
    public StudentRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }

    public void CreateStudentAsync(Student student) => Create(student);

    public void DeleteStudentAsync(Student student) => Delete(student);

    public async Task<PagedList<Student>>? GetAllStudentsAsync(RequestParameters requestParameters, bool trackChanges)
    {
        var students = await FindByCondition(s => s.User!.IsActive == requestParameters.IsActive, trackChanges)
            .Search(requestParameters.SearchTerm!)
            .Sort(requestParameters.OrderBy!)
            .Include(s => s.User)
            .ToListAsync();

        return PagedList<Student>.ToPagedList(students, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<Student?> GetStudentAsync(Guid id, bool trackChanges) =>
        await FindByCondition(s => s.UserId == id, trackChanges)
        .Include(s => s.User)
        .SingleOrDefaultAsync();

    public void UpdateStudentAsync(Student student) => Update(student);
}
