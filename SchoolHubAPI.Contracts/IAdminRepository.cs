using SchoolHubAPI.Entities.Entities;

namespace SchoolHubAPI.Contracts;

public interface IAdminRepository
{
    Task<List<Admin>>? GetAllAdminsAsync(bool trackChanges);
    Task<Admin?> GetAdminAsync(Guid id, bool trackChanges);
    void DeleteAsminAsync(Admin admin);
    void CreateAdminAsync(Admin admin);
    void UpdateAdminAsync(Admin admin);
}
