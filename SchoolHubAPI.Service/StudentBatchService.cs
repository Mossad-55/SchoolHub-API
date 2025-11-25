using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.DTOs.StudentBatch;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class StudentBatchService : IStudentBatchService
{
    private readonly IRepositoryManager _repository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _logger;

    public StudentBatchService(IRepositoryManager repository, UserManager<User> userManager, 
        IMapper mapper, ILoggerManager loggerManager)
    {
        _repository = repository;
        _userManager = userManager;
        _mapper = mapper;
        _logger = loggerManager;
    }

    public async Task EnrollAsync(Guid batchId, Guid studentId, bool batchTrackChanges, bool sbTrackChanges)
    {
        _logger.LogInfo($"Enrolling student with id: {studentId} in batch {batchId}.");

        await EnsureBatchExists(batchId, batchTrackChanges);

        await EnsureStudentExistsWithRole(studentId);

        await EnsureStudentNotEnrolledInBatch(studentId, batchId, sbTrackChanges);

        var studentBatchEntity = new StudentBatch
        {
            BatchId = batchId,
            StudentId = studentId,
            EnrollmentDate = DateTime.UtcNow
        };

        _repository.StudentBatch.Enroll(studentBatchEntity);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Student with id: {studentId} enrolled in batch {batchId}.");
    }

    public async Task<(IEnumerable<StudentBatchDto> StudentBatchDtos, MetaData MetaData)> GetAllAsync(Guid batchId, RequestParameters requestParameters, bool batchTrackChanges, bool sbTrackChanges)
    {
        _logger.LogInfo($"Retrieving student enrollments for batch {batchId}.");

        await EnsureBatchExists(batchId, batchTrackChanges);

        var studentBatchesWithMetaData = await _repository.StudentBatch.GetStudentBatchesAsync(batchId, requestParameters, sbTrackChanges);

        var studentBatchDtos = _mapper.Map<IEnumerable<StudentBatchDto>>(studentBatchesWithMetaData);

        return (studentBatchDtos, studentBatchesWithMetaData.MetaData);
    }

    public async Task<(IEnumerable<BatchForStudentDto> StudentBatchDtos, MetaData MetaData)> GetAllForStudentAsync(Guid studentId, RequestParameters requestParameters, bool sbTrackChanges)
    {
        _logger.LogInfo($"Retrieving batches for student with id: {studentId}.");

        var studentBatchesWithMetaData = await _repository.StudentBatch.GetBatchesForStudentAsync(studentId, requestParameters, sbTrackChanges);

        var batchesForStudentDto = _mapper.Map<IEnumerable<BatchForStudentDto>>(studentBatchesWithMetaData);

        return (batchesForStudentDto, studentBatchesWithMetaData.MetaData);
    }

    public async Task<StudentBatchDto?> GetByIdForBatchAsync(Guid batchId, Guid id, bool batchTrackChanges, bool sbTrackChanges)
    {
        _logger.LogInfo($"Retrieving enrollment {id} for batch {batchId}.");

        await EnsureBatchExists(batchId, batchTrackChanges);

        var studentBatchEntity = await _repository.StudentBatch.GetByIdForBatchAsync(batchId, id, sbTrackChanges);

        return _mapper.Map<StudentBatchDto>(studentBatchEntity);
    }

    public async Task RemoveAsync(Guid batchId, Guid studentId, bool batchTrackChanges, bool sbTrackChanges)
    {
        _logger.LogInfo($"Removing student with id: {studentId} from batch {batchId}.");

        await EnsureBatchExists(batchId, batchTrackChanges);

        var studentBatchEntity = await _repository.StudentBatch.GetByIdForBatchAsync(batchId, studentId, sbTrackChanges);
        if (studentBatchEntity is null)
            throw new StudentNotInBatchException(studentId, batchId);

        _repository.StudentBatch.Remove(studentBatchEntity);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Student with id: {studentId} is removed from batch {batchId}.");
    }

    // Private Functions
    private async Task EnsureBatchExists(Guid batchId, bool trackChanges)
    {
        var batchEntity = await _repository.Batch.GetBatchByIdAsync(batchId, trackChanges);
        if (batchEntity is null)
        {
            _logger.LogWarn($"Batch with id: {batchId} not found.");
            throw new BatchNotFoundException(batchId);
        }

        _logger.LogDebug($"Batch with id: {batchId} exists.");
    }

    private async Task EnsureStudentExistsWithRole(Guid studentId)
    {
        var user = await _userManager.FindByIdAsync(studentId.ToString());
        if(user is null)
        {
            _logger.LogWarn($"Student with id: {studentId} not found.");
            throw new UserNotFoundException(studentId);
        }

        var requiredRole = RolesEnum.Student.ToString();
        if(!await _userManager.IsInRoleAsync(user, requiredRole))
        {
            _logger.LogWarn($"User with id: {studentId} is not a Student.");
            throw new UserNotInRoleException(studentId);
        }

        _logger.LogDebug($"Student with id: {studentId} exists.");
    }

    private async Task EnsureStudentNotEnrolledInBatch(Guid studentId, Guid batchId, bool trackChanges)
    {
        if(await _repository.StudentBatch.ExistsAsync(studentId, batchId, trackChanges))
        {
            _logger.LogWarn($"Student with id: {studentId} is enrolled in batch {batchId}.");
            throw new StudentIsEnrolledException(studentId);
        }

        _logger.LogInfo($"Student with id: {studentId} is not enrolled.");
    }
}
