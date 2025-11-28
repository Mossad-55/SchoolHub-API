using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface INotificationRepository
{
    Task<PagedList<Notification>> GetAllAsync(RequestParameters requestParameters, bool trackChanges);
    Task<PagedList<Notification>> GetForRecipientAsync(RecipientRole role, RequestParameters requestParameters, bool trackChanges, Guid? recipientId = null);
    Task<Notification?> GetByIdAsync(Guid id, bool trackChanges);
    void AddNotification(Notification notification);
    void RemoveNotification(Notification notification);
}
