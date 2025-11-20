using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface IDepartmentRepository
{
    Task<PagedList<Department>>? GetAllDepartmentsAsync(RequestParameters requestParameters, bool trackChanges);
    Task<Department?> GetDepartmentAsync(Guid id, bool trackChanges);
    Task<bool> CheckIfDepatmentExists(string name, bool trackChanges);
    Task<bool> CheckIfTeacherIsHeadOfDepartment(Guid teacherId, bool trackChanges);
    void DeleteDepartment(Department department);
    void CreateDepartment(Department department);
    void UpdateDepartment(Department department);
}
