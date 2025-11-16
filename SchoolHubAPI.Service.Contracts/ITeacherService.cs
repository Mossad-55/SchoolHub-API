using SchoolHubAPI.Shared.DTOs.Teacher;

namespace SchoolHubAPI.Service.Contracts;

public interface ITeacherService
{
    Task<IEnumerable<TeacherDto>>? GetAllAsync(bool trackChanges);
    Task<TeacherDto?> GetByIdAsync(Guid id, bool trackChanges);
    Task CreateAsync(Guid userId, bool trackChanges);
}
