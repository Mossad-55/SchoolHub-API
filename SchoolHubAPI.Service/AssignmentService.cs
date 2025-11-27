using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.DTOs.Assignment;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class AssignmentService : IAssignmentService
{
    private readonly IRepositoryManager _repository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _logger;

    public AssignmentService(IRepositoryManager repository, UserManager<User> userManager, IMapper mapper, ILoggerManager logger)
    {
        _repository = repository;
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AssignmentDto?> CreateAsync(Guid batchId, Guid teacherId, AssignmentForCreationDto creationDto, bool batchTrachChanges, bool assignmentTrackChanges)
    {
        await EnsureBatchExistsAsync(batchId, batchTrachChanges);

        await EnsureTeacherExistsWithRole(teacherId);

        var assignmentEntity = _mapper.Map<Assignment>(creationDto);
        assignmentEntity.BatchId = batchId;
        assignmentEntity.CreatedByTeacherId = teacherId;

        _repository.Assignment.CreateAssignment(assignmentEntity);

        await _repository.SaveChangesAsync();

        var assigmentDto = _mapper.Map<AssignmentDto>(assignmentEntity);

        return assigmentDto;
    }

    public async Task DeleteAsync(Guid batchId, Guid id, bool batchTrachChanges, bool assignmentTrackChanges)
    {
        await EnsureBatchExistsAsync(batchId, batchTrachChanges);

        var assignmentEntity = await GetAssignmentForBatch(batchId, id, assignmentTrackChanges);
        
        _repository.Assignment.DeleteAssignment(assignmentEntity);

        await _repository.SaveChangesAsync();
    }

    public async Task<(IEnumerable<AssignmentDto> AssignmentDtos, MetaData MetaData)> GetAllForBatchAsync(Guid batchId, RequestParameters requestParameters, bool batchTrackChanges, bool assignmentTrackChanges)
    {
        await EnsureBatchExistsAsync(batchId, batchTrackChanges);

        var assignmentWithMetaData = await _repository.Assignment.GetAllForBatchAsync(batchId, requestParameters, assignmentTrackChanges);

        var assignmentDtos = _mapper.Map<IEnumerable<AssignmentDto>>(assignmentWithMetaData);

        return (assignmentDtos, assignmentWithMetaData.MetaData);
    }

    public async Task<AssignmentDto?> GetByIdAsync(Guid id, bool trackChanges)
    {
        var assignmentEntity = await _repository.Assignment.GetByIdAsync(id, trackChanges);
        if (assignmentEntity is null)
            throw new AssignmentNotFoundException(id);

        return _mapper.Map<AssignmentDto>(assignmentEntity);
    }

    public async Task<AssignmentDto> GetForBatchByIdAsync(Guid batchId, Guid id, bool batchTrachChanges, bool assignmentTrackChanges)
    {
        await EnsureBatchExistsAsync(batchId, batchTrachChanges);

        var assigmentEntity = await GetAssignmentForBatch(batchId, id, assignmentTrackChanges);

        return _mapper.Map<AssignmentDto>(assigmentEntity);
    }

    public async Task UpdateAsync(Guid batchId, Guid teacherId, Guid id, AssignmentForUpdateDto updateDto, bool batchTrachChanges, bool assignmentTrackChanges)
    {
        await EnsureBatchExistsAsync(batchId, batchTrachChanges);

        await EnsureTeacherExistsWithRole(teacherId);

        var assignmentEntity = await GetAssignmentForBatch(batchId, id, assignmentTrackChanges);

        _mapper.Map(updateDto, assignmentEntity);

        await _repository.SaveChangesAsync();
    }

    // Private Functions
    private async Task EnsureBatchExistsAsync(Guid batchId, bool trackChanges)
    {
        var batch = await _repository.Batch.GetBatchByIdAsync(batchId, trackChanges);
        if (batch is null)
        {
            _logger.LogWarn($"Batch with id: {batchId} not found.");
            throw new BatchNotFoundException(batchId);
        }

        _logger.LogDebug($"Batch with id: {batchId} exists.");
    }

    private async Task<Assignment> GetAssignmentForBatch(Guid batchId, Guid id, bool trackChanges)
    {
        var assignmentEntity = await _repository.Assignment.GetForBatchByIdAsync(batchId, id, trackChanges);
        if (assignmentEntity is null)
        {
            _logger.LogWarn($"Assignment with id: {id} in Batch {batchId} not found.");
            throw new AssignmentNotFoundException(id);
        }

        _logger.LogDebug($"Assignment with id: {id} retrieved for batch {batchId}.");
        return assignmentEntity;
    }

    private async Task EnsureTeacherExistsWithRole(Guid teacherId)
    {
        var teacher = await _userManager.FindByIdAsync(teacherId.ToString());
        if(teacher is null)
            throw new UserNotFoundException(teacherId);

        if (await _userManager.IsInRoleAsync(teacher, RolesEnum.Teacher.ToString()))
            throw new UserNotInRoleException(teacherId);
    }
}
