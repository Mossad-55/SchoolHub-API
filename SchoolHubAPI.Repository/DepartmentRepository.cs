using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Repository;

internal sealed class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
{
    public DepartmentRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }

    public async Task<bool> CheckIfDepatmentExists(string name, bool trackChanges) =>
        await FindByCondition(d => d.Name == name, trackChanges)
        .SingleOrDefaultAsync() != null;

    public async Task<bool> CheckIfTeacherIsHeadOfDepartment(Guid teacherId, bool trackChanges) =>
        await FindByCondition(d => d.HeadOfDepartmentId == teacherId, trackChanges)
        .SingleOrDefaultAsync() != null;

    public void CreateDepartment(Department department) => Create(department);

    public void DeleteDepartment(Department department) => Delete(department);

    public async Task<PagedList<Department>>? GetAllDepartmentsAsync(RequestParameters requestParameters, bool trackChanges)
    {
        var departments = await FindAll(trackChanges)
            .Search(requestParameters.SearchTerm!)
            .Sort(requestParameters.OrderBy!)
            .Include(d => d.HeadOfDepartment)
            .ThenInclude(t => t!.User)
            .ToListAsync();

        return PagedList<Department>.ToPagedList(departments, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<Department?> GetDepartmentAsync(Guid id, bool trackChanges) =>
        await FindByCondition(d => d.Id == id, trackChanges)
        .Include(d => d.HeadOfDepartment)
        .ThenInclude(t => t!.User)
        .SingleOrDefaultAsync();

    public void UpdateDepartment(Department department) => Update(department);
}
