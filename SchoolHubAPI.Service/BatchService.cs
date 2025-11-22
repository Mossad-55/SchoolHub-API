using AutoMapper;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
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
        batchEntity.TeacherId = teacherId; // Don't need to check for Role or Existance here, (handled in controller for(Role, Token))

        _repository.Batch.CreateBatch(batchEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Batch created with id: {batchEntity.Id} in course {batchEntity.CourseId}.");

        return _mapper.Map<BatchDto>(batchEntity);
    }

    public async Task DeleteAsync(Guid courseId, Guid id, bool courseTrackChanges, bool batchTrackChanges)
    {
        _logger.LogInfo($"Deleting batch {id} from course {courseId}.");

        await EnsureCourseExistsAsync(courseId, courseTrackChanges);

        var batchEntity = await GetBatchForCourse(courseId, id, batchTrackChanges);

        _repository.Batch.DeleteBatch(batchEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Batch {id} deleted from course {courseId}.");
    }

    public async Task<(IEnumerable<BatchDto> BatchDtos, MetaData MetaData)> GetAllAsync(Guid courseId, RequestParameters requestParameters, bool courseTrackChanges, bool batchTrackChanges)
    {
        _logger.LogInfo($"Retrieving batches for course {courseId} - page {requestParameters.PageNumber}, size {requestParameters.PageSize}.");

        await EnsureCourseExistsAsync(courseId, courseTrackChanges);

        var batchEntitiesWithMetaData = await _repository.Batch.GetAllBatches(courseId, requestParameters, batchTrackChanges);

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

    public async Task UpdateAsync(Guid courseId, Guid id, BatchForUpdateDto updateDto, bool courseTrackChanges, bool batchTrackChanges)
    {
        _logger.LogInfo($"Updating batch {id} in course {courseId}.");

        await EnsureCourseExistsAsync(courseId, courseTrackChanges);

        var batchEntity = await GetBatchForCourse(courseId, id, batchTrackChanges);

        var normalizedName = updateDto.Name?.Trim().ToUpperInvariant() ?? string.Empty;
        await EnsureBatchNameIsUniqueAsync(courseId, batchEntity.TeacherId, normalizedName, batchTrackChanges, batchEntity.Name);

        _mapper.Map(updateDto, batchEntity);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Batch {id} updated in course {courseId}.");
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
