using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Repository;

internal sealed class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
{
    public NotificationRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public void AddNotification(Notification notification) => Create(notification);

    public async Task<PagedList<Notification>> GetAllAsync(RequestParameters requestParameters, bool trackChanges)
    {
        var notifications = await FindAll(trackChanges)
            .Search(requestParameters.SearchTerm!)
            .Sort(requestParameters.OrderBy!)
            .ToListAsync();

        return PagedList<Notification>.ToPagedList(notifications, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<Notification?> GetByIdAsync(Guid id, bool trackChanges) =>
        await FindByCondition(n => n.Id == id, trackChanges)
            .SingleOrDefaultAsync();

    public async Task<PagedList<Notification>> GetForRecipientAsync(RecipientRole role, RequestParameters requestParameters, bool trackChanges, Guid? recipientId = null)
    {
        var notifications = await FindByCondition(n => n.RecipientRole == role &&
            (n.RecipientId == null || n.RecipientId == recipientId), trackChanges)
            .Search(requestParameters.SearchTerm!)
            .Sort(requestParameters.OrderBy!)
            .ToListAsync();

        return PagedList<Notification>.ToPagedList(notifications, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public void RemoveNotification(Notification notification) => Delete(notification);
}
