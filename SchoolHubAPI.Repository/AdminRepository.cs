using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;

namespace SchoolHubAPI.Repository;

internal sealed class AdminRepository : RepositoryBase<Admin>, IAdminRepository
{
    public AdminRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }

    public void CreateAdminAsync(Admin admin) => Create(admin);

    public void DeleteAsminAsync(Admin admin) => Delete(admin);

    public async Task<Admin?> GetAdminAsync(Guid id, bool trackChanges) =>
        await FindByCondition(a => a.UserId == id, trackChanges)
        .Include(a => a.User)
        .SingleOrDefaultAsync();

    public async Task<List<Admin>>? GetAllAdminsAsync(bool trackChanges) =>
        await FindAll(trackChanges)
        .Include(a => a.User)
        .ToListAsync();

    public void UpdateAdminAsync(Admin admin) => Update(admin);
}
