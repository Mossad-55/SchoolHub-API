using SchoolHubAPI.Shared.DTOs.Course;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface ICourseService
{
    Task<(IEnumerable<CourseDto> CourseDtos, MetaData MetaData)> GetAllAsync(Guid departmentId, RequestParameters requestParameters, bool trackChanges);
    Task<CourseDto?> GetByIdForDepartmentAsync(Guid departmentId, Guid id, bool trackChanges);
    Task<CourseDto?> CreateAsync(CourseForCreationDto creationDto, bool trackChanges);
    Task UpdateAsync(Guid departmentId, Guid id, CourseForUpdateDto updateDto, bool trackChanges);
    Task DeleteAsync(Guid departmentId, Guid id, bool trackChanges);
}
