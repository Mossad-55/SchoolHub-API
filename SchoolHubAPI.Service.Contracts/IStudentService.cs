using SchoolHubAPI.Shared.DTOs.Student;

namespace SchoolHubAPI.Service.Contracts;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>>? GetAllAsync(bool trackChanges);
    Task<StudentDto?> GetByIdAsync(Guid id, bool trackChanges);
    Task CreateAsync(Guid userId, bool trackChanges);
}
