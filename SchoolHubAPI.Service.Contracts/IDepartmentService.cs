using SchoolHubAPI.Shared.DTOs.Department;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface IDepartmentService
{
    Task<(IEnumerable<DepartmentDto> departmentDtos, MetaData MetaData)> GetAllAsync(RequestParameters requestParameters, bool trackChanges);
    Task<DepartmentDto?> GetByIdAsync(Guid id, bool trackChanges);
    Task<DepartmentDto?> CreateAsync(DepartmentForCreationDto creationDto, bool trackChanges);
    Task UpdateAsync(Guid id, DepartmentForUpdateDto updateDto, bool trackChanges);
    Task DeleteAsync(Guid id, bool trackChanges);
    Task<bool> IsTeacherHeadOfDepartment(Guid teacherId, bool trackChanges);
}
