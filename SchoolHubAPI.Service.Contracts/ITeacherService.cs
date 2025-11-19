using SchoolHubAPI.Shared.DTOs.Teacher;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface ITeacherService
{
    Task<(IEnumerable<TeacherDto> TeacherDtos, MetaData MetaData)> GetAllAsync(RequestParameters requestParameters, bool trackChanges);
    Task<TeacherDto?> GetByIdAsync(Guid id, bool trackChanges);
    Task CreateAsync(Guid userId, bool trackChanges);
}
