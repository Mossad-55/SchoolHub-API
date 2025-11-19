using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface ITeacherRepository
{
    Task<PagedList<Teacher>>? GetAllTeachersAsync(RequestParameters requestParameters, bool trackChanges);
    Task<Teacher?> GetTeacherAsync(Guid id, bool trackChanges);
    void DeleteTeacherAsync(Teacher teacher);
    void CreateTeacherAsync(Teacher teacher);
    void UpdateTeacherAsync(Teacher teacher);
}
