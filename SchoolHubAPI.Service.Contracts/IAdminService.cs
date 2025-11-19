using SchoolHubAPI.Shared.DTOs.Admin;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface IAdminService
{
    Task<(IEnumerable<AdminDto> AdminDtos, MetaData MetaData)> GetAllAsync(RequestParameters requestParameters,bool trackChanges);
    Task<AdminDto?> GetByIdAsync(Guid id, bool trackChanges);
    Task CreateAsync(Guid userId, bool trackChanges);
}
