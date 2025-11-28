using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.DTOs.Notification;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service.Contracts;

public interface INotificationService
{
    Task<(IEnumerable<NotificationDto> NotificationDtos, MetaData MetaData)> GetForUserAsync(RecipientRole role, RequestParameters requestParameters, bool trackChanges, Guid? userId = null);
    Task<NotificationDto?> GetByIdAsync(Guid id, RecipientRole role, bool trackChanges, Guid? userId = null);
    Task<NotificationDto> CreateAsync(NotificationForCreationDto dto);
    Task UpdateAsync(Guid id, NotificationForUpdateDto dto, bool trackChanges);
    Task MarkAsReadAsync(Guid id, RecipientRole role, bool trackChanges, Guid? userId = null);
    Task DeleteAsync(Guid id, bool trackChanges);
}
