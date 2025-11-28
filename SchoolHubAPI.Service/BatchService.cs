using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.DTOs.Batch;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class BatchService : IBatchService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _logger;

    public BatchService(IRepositoryManager repositoryManager, IMapper mapper, ILoggerManager loggerManager)
    {
        _repository = repositoryManager;
        _mapper = mapper;
        _logger = loggerManager;
    }

    public async Task<BatchDto?> CreateAsync(Guid courseId, Guid teacherId, BatchForCreationDto creationDto, bool courseTrackChanges, bool batchTrackChanges)
    {
        _logger.LogInfo($"Creating batch in course {courseId} by teacher {teacherId}.");

        await EnsureCourseExistsAsync(courseId, courseTrackChanges);

        var normalizedName = creationDto.Name?.Trim().ToUpperInvariant() ?? string.Empty;
        await EnsureBatchNameIsUniqueAsync(courseId, teacherId, normalizedName, batchTrackChanges);

        var batchEntity = _mapper.Map<Batch>(creationDto);
        batchEntity.CourseId = courseId;
        batchEntity.TeacherId = teacherId;

        _repository.Batch.CreateBatch(batchEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Batch created with id: {batchEntity.Id} in course {batchEntity.CourseId}.");

        // --- Notifications ---
        var adminNotification = new Notification
        {
            Title = "Batch Created",
            Message = $"New batch '{batchEntity.Name}' created in course '{courseId}'.",
            RecipientRole = RecipientRole.Admin,
            CreatedDate = DateTime.UtcNow
        };
        _repository.Notification.AddNotification(adminNotification);
        await _repository.SaveChangesAsync();
        // -------------------

        return _mapper.Map<BatchDto>(batchEntity);
    }

    public async Task UpdateAsync(Guid courseId, Guid teacherId, Guid id, BatchForUpdateDto updateDto, bool courseTrackChanges, bool batchTrackChanges)
    {
        _logger.LogInfo($"Updating batch {id} in course {courseId}.");

        await EnsureCourseExistsAsync(courseId, courseTrackChanges);

        var batchEntity = await GetBatchForCourse(courseId, id, batchTrackChanges);
        if (batchEntity.TeacherId != teacherId)
            throw new BatchNotAssignedToTeacherException(id);

        var normalizedName = updateDto.Name?.Trim().ToUpperInvariant() ?? string.Empty;
        await EnsureBatchNameIsUniqueAsync(courseId, batchEntity.TeacherId, normalizedName, batchTrackChanges, batchEntity.Name);

        _mapper.Map(updateDto, batchEntity);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Batch {id} updated in course {courseId}.");

        // --- Notifications ---
        var adminNotification = new Notification
        {
            Title = "Batch Updated",
            Message = $"Batch '{batchEntity.Name}' updated in course '{courseId}'.",
            RecipientRole = RecipientRole.Admin,
            CreatedDate = DateTime.UtcNow
        };
        _repository.Notification.AddNotification(adminNotification);
        await _repository.SaveChangesAsync();
        // -------------------
    }

    public async Task DeleteAsync(Guid courseId, Guid teacherId, Guid id, bool courseTrackChanges, bool batchTrackChanges)
    {
        _logger.LogInfo($"Deleting batch {id} from course {courseId}.");

        await EnsureCourseExistsAsync(courseId, courseTrackChanges);

        var batchEntity = await GetBatchForCourse(courseId, id, batchTrackChanges);
        if (batchEntity.TeacherId != teacherId)
            throw new BatchNotAssignedToTeacherException(id);

        _repository.Batch.DeleteBatch(batchEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Batch {id} deleted from course {courseId}.");

        // --- Notifications ---
        var adminNotification = new Notification
        {
            Title = "Batch Deleted",
            Message = $"Batch '{batchEntity.Name}' deleted from course '{courseId}'.",
            RecipientRole = RecipientRole.Admin,
            CreatedDate = DateTime.UtcNow
        };
        _repository.Notification.AddNotification(adminNotification);
        await _repository.SaveChangesAsync();
        // -------------------
    }

    public async Task ActivateAsync(Guid courseId, Guid teacherId, Guid id, bool courseTrackChanges, bool batchTrackChanges)
    {
        _logger.LogInfo($"Activating batch {id} for course {courseId}.");

        await EnsureCourseExistsAsync(courseId, courseTrackChanges);

        var batchEntity = await GetBatchForCourse(courseId, id, batchTrackChanges);
        if (batchEntity.TeacherId != teacherId)
            throw new BatchNotAssignedToTeacherException(id);

        if (batchEntity.IsActive)
            throw new BatchStatusException("activated");

        batchEntity.IsActive = true;
        batchEntity.UpdatedDate = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Batch {id} activated for course {courseId}.");

        // --- Notifications ---
        var adminNotification = new Notification
        {
            Title = "Batch Activated",
            Message = $"Batch '{batchEntity.Name}' activated in course '{courseId}'.",
            RecipientRole = RecipientRole.Admin,
            CreatedDate = DateTime.UtcNow
        };
        _repository.Notification.AddNotification(adminNotification);
        await _repository.SaveChangesAsync();
        // -------------------
    }

    public async Task DeActivateAsync(Guid courseId, Guid teacherId, Guid id, bool courseTrackChanges, bool batchTrackChanges)
    {
        _logger.LogInfo($"Deactivating batch {id} for course {courseId}.");

        await EnsureCourseExistsAsync(courseId, courseTrackChanges);

        var batchEntity = await GetBatchForCourse(courseId, id, batchTrackChanges);
        if (batchEntity.TeacherId != teacherId)
            throw new BatchNotAssignedToTeacherException(id);

        if (!batchEntity.IsActive)
            throw new BatchStatusException("deactivated");

        batchEntity.IsActive = false;
        batchEntity.UpdatedDate = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Batch {id} deactivated for course {courseId}.");

        // --- Notifications ---
        var adminNotification = new Notification
        {
            Title = "Batch Deactivated",
            Message = $"Batch '{batchEntity.Name}' deactivated in course '{courseId}'.",
            RecipientRole = RecipientRole.Admin,
            CreatedDate = DateTime.UtcNow
        };
        _repository.Notification.AddNotification(adminNotification);
        await _repository.SaveChangesAsync();
        // -------------------
    }

    public async Task<(IEnumerable<BatchDto> BatchDtos, MetaData MetaData)> GetAllAsync(Guid courseId, RequestParameters requestParameters, bool courseTrackChanges, bool batchTrackChanges)
    {
        _logger.LogInfo($"Retrieving batches for course {courseId} - page {requestParameters.PageNumber}, size {requestParameters.PageSize}.");

        await EnsureCourseExistsAsync(courseId, courseTrackChanges);

        var batchEntitiesWithMetaData = await _repository.Batch.GetAllBatchesForCourse(courseId, requestParameters, batchTrackChanges);

        var batchDtos = _mapper.Map<IEnumerable<BatchDto>>(batchEntitiesWithMetaData);

        return (batchDtos, batchEntitiesWithMetaData.MetaData);
    }

    public async Task<BatchDto?> GetByIdForCourseAsync(Guid courseId, Guid id, bool courseTrackChanges, bool batchTrackChanges)
    {
        _logger.LogInfo($"Retrieving batch {id} for course {courseId}.");

        await EnsureCourseExistsAsync(courseId, courseTrackChanges);

        var batchEntity = await GetBatchForCourse(courseId, id, batchTrackChanges);

        var batchDto = _mapper.Map<BatchDto>(batchEntity);

        _logger.LogInfo($"Batch {id} retrieved for course {courseId}.");

        return batchDto;
    }

    public async Task<(IEnumerable<BatchDto> BatchDtos, MetaData MetaData)> GetAllAsyncForTeacher(Guid teacherId, RequestParameters requestParameters, bool batchTrackChanges)
    {
        _logger.LogInfo($"Retrieving batches for teacher {teacherId} - page {requestParameters.PageNumber}, size {requestParameters.PageSize}.");

        var batchEntitiesWithMetaData = await _repository.Batch.GetAllBatchesForTeacher(teacherId, requestParameters, batchTrackChanges);

        var batchDtos = _mapper.Map<IEnumerable<BatchDto>>(batchEntitiesWithMetaData);

        return (batchDtos, batchEntitiesWithMetaData.MetaData);
    }

    // Private Functions
    private async Task EnsureCourseExistsAsync(Guid courseId, bool trackChanges)
    {
        var course = await _repository.Course.GetCourseByIdAsync(courseId, trackChanges);
        if (course is null)
        {
            _logger.LogWarn($"Course with id: {courseId} not found.");
            throw new CourseNotFoundException(courseId);
        }

        _logger.LogDebug($"Course with id: {courseId} exists.");
    }

    private async Task<Batch> GetBatchForCourse(Guid courseId, Guid id, bool trackChanges)
    {
        var batchEntity = await _repository.Batch.GetBatchForCourseAsync(courseId, id, trackChanges);
        if (batchEntity is null)
        {
            _logger.LogWarn($"Batch with id: {id} in Course {courseId} not found.");
            throw new BatchNotFoundException(id);
        }

        _logger.LogDebug($"Batch with id: {id} retrieved for course {courseId}.");
        return batchEntity;
    }

    private async Task EnsureBatchNameIsUniqueAsync(Guid courseId, Guid teacherId, string name, bool trackChanges, string? currentName = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (await _repository.Batch.CheckIfBatchExistsAsync(courseId, teacherId, name, trackChanges)
            && !string.Equals(name, currentName, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarn($"Batch with name '{name}' already exists in course {courseId}.");
            throw new BatchExistsException(name);
        }

        _logger.LogDebug($"Batch name '{name}' is unique in course {courseId} or unchanged.");
    }
}
