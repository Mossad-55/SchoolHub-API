using SchoolHubAPI.Contracts;

namespace SchoolHubAPI.Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    // Repositories 

    // Common Methods
    public async Task SaveChangesAsync() => await _repositoryContext.SaveChangesAsync();
}
