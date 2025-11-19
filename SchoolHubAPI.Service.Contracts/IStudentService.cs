using SchoolHubAPI.Shared.DTOs.Student;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface IStudentService
{
    Task<(IEnumerable<StudentDto> StudentDtos, MetaData MetaData)> GetAllAsync(RequestParameters requestParameters, bool trackChanges);
    Task<StudentDto?> GetByIdAsync(Guid id, bool trackChanges);
    Task CreateAsync(Guid userId, bool trackChanges);
}
