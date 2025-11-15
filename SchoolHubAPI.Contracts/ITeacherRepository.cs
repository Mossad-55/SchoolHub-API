using SchoolHubAPI.Entities.Entities;

namespace SchoolHubAPI.Contracts;

public interface ITeacherRepository
{
    Task<List<Teacher>>? GetAllTeachersAsync(bool trackChanges);
    Task<Teacher?> GetTeacherAsync(Guid id, bool trackChanges);
    void DeleteTeacherAsync(Teacher teacher);
    void CreateTeacherAsync(Teacher teacher);
    void UpdateTeacherAsync(Teacher teacher);
}
