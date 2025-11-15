using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;

namespace SchoolHubAPI.Repository;

internal sealed class StudentRepository : RepositoryBase<Student>, IStudentRepository
{
    public StudentRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }

    public void CreateStudentAsync(Student student) => Create(student);

    public void DeleteStudentAsync(Student student) => Delete(student);

    public async Task<List<Student>>? GetAllStudentsAsync(bool trackChanges) =>
        await FindAll(trackChanges)
        .Include(s => s.User)
        .ToListAsync();

    public async Task<Student?> GetStudentAsync(Guid id, bool trackChanges) =>
        await FindByCondition(s => s.UserId == id, trackChanges)
        .Include(s => s.User)
        .SingleOrDefaultAsync();

    public void UpdateStudentAsync(Student student) => Update(student);
}
