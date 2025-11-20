using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface ICourseRepository
{
    Task<PagedList<Course>>? GetAllCoursesAsync(RequestParameters requestParameters, bool trackChanges);
    Task<Course?> GetCourseAsync(Guid id, bool trackChanges);
    Task<bool> CheckIfCourseExists(Guid departmentId, string code, bool trackChanges);
    void DeleteCourse(Course course);
    void CreateCourse(Course course);
    void UpdateCourse(Course course);
}
