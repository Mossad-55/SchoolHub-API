using AutoMapper;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.DTOs.Notification;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class NotificationService : INotificationService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _logger;

    public NotificationService(IRepositoryManager repository, IMapper mapper, ILoggerManager logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<NotificationDto> CreateAsync(NotificationForCreationDto dto)
    {
        var notificationEntity = _mapper.Map<Notification>(dto);

        _repository.Notification.AddNotification(notificationEntity);

        await _repository.SaveChangesAsync();

        return _mapper.Map<NotificationDto>(notificationEntity);
    }

    public async Task DeleteAsync(Guid id, bool trackChanges)
    {
        var notificationEntity = await _repository.Notification.GetByIdAsync(id, trackChanges);
        if (notificationEntity is null)
            throw new NotificationNotFoundException();

        _repository.Notification.RemoveNotification(notificationEntity);

        await _repository.SaveChangesAsync();
    }

    public async Task<NotificationDto?> GetByIdAsync(Guid id, RecipientRole role, bool trackChanges, Guid? userId = null)
    {
        var notificationEntity = await _repository.Notification.GetByIdAsync(id, trackChanges);
        if (notificationEntity is null)
            throw new NotificationNotFoundException();

        if (role != RecipientRole.Admin && !(notificationEntity.RecipientRole == role) &&
            (notificationEntity.RecipientId == null || (userId.HasValue && notificationEntity.RecipientId == userId)))
            throw new NotificationNotFoundException();

        return _mapper.Map<NotificationDto>(notificationEntity);
    }

    public async Task<(IEnumerable<NotificationDto> NotificationDtos, MetaData MetaData)> GetForUserAsync(RecipientRole role, RequestParameters requestParameters, bool trackChanges, Guid? userId = null)
    {
        if(role == RecipientRole.Admin)
        {
            var notificationsWithMetaDataForAdmin = await _repository.Notification.GetAllAsync(requestParameters, trackChanges);

            var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notificationsWithMetaDataForAdmin);

            return (notificationDtos, notificationsWithMetaDataForAdmin.MetaData);
        }

        var notificationsWithMetaData = await _repository.Notification.GetForRecipientAsync(role, requestParameters, trackChanges, userId);

        var notifcationsDtos = _mapper.Map<IEnumerable<NotificationDto>>(notificationsWithMetaData);

        return (notifcationsDtos, notificationsWithMetaData.MetaData);
    }

    public async Task MarkAsReadAsync(Guid id, RecipientRole role, bool trackChanges, Guid? userId = null)
    {
        var notificationEntity = await _repository.Notification.GetByIdAsync(id, trackChanges);
        if (notificationEntity is null)
            throw new NotificationNotFoundException();

        if (role != RecipientRole.Admin && !(notificationEntity.RecipientRole == role) &&
            (notificationEntity.RecipientId == null || (userId.HasValue && notificationEntity.RecipientId == userId)))
            throw new NotificationNotFoundException();

        notificationEntity.IsRead = true;

        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, NotificationForUpdateDto dto, bool trackChanges)
    {
        var notificationEntity = await _repository.Notification.GetByIdAsync(id, trackChanges);
        if (notificationEntity is null)
            throw new NotificationNotFoundException();

        _mapper.Map(dto, notificationEntity);

        await _repository.SaveChangesAsync();
    }
}
