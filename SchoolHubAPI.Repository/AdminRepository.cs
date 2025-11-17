using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared.RequestFeatures;

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

    public async Task<PagedList<Admin>>? GetAllAdminsAsync(RequestParameters requestParameters, bool trackChanges)
    {
        var admins = await FindAll(trackChanges)
            .Search(requestParameters.SearchTerm!)
            .Sort(requestParameters.OrderBy!)
            .Include(a => a.User)
            .ToListAsync();

        return PagedList<Admin>.ToPagedList(admins, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public void UpdateAdminAsync(Admin admin) => Update(admin);
}
