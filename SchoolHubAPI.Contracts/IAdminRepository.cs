using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface IAdminRepository
{
    Task<PagedList<Admin>>? GetAllAdminsAsync(RequestParameters requestParameters, bool trackChanges);
    Task<Admin?> GetAdminAsync(Guid id, bool trackChanges);
    void DeleteAsminAsync(Admin admin);
    void CreateAdminAsync(Admin admin);
    void UpdateAdminAsync(Admin admin);
}
