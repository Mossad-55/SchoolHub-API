using SchoolHubAPI.Shared.DTOs.Course;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface ICourseService
{
    Task<(IEnumerable<CourseDto> CourseDtos, MetaData MetaData)> GetAllAsync(Guid departmentId, RequestParameters requestParameters, bool depTrackChanges, bool courseTrackChanges);
    Task<CourseDto?> GetByIdForDepartmentAsync(Guid departmentId, Guid id, bool depTrackChanges, bool courseTrackChanges);
    Task<CourseDto?> CreateAsync(CourseForCreationDto creationDto, bool depTrackChanges, bool courseTrackChanges);
    Task UpdateAsync(Guid departmentId, Guid id, CourseForUpdateDto updateDto, bool depTrackChanges, bool courseTrackChanges);
    Task DeleteAsync(Guid departmentId, Guid id, bool depTrackChanges, bool courseTrackChanges);
}
