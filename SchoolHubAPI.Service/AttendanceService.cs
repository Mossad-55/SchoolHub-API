using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.DTOs.Attendance;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class AttendanceService : IAttendanceService
{
    private readonly IRepositoryManager _repository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _logger;

    public AttendanceService(IRepositoryManager repository, UserManager<User> userManager,
        IMapper mapper, ILoggerManager loggerManager)
    {
        _repository = repository;
        _userManager = userManager;
        _mapper = mapper;
        _logger = loggerManager;
    }

    public async Task<AttendanceDto?> CreateAttendanceAsync(Guid batchId, Guid teacherId, AttendanceForCreationDto creationDto, bool batchTrackChanges, bool attTrackChanges)
    {
        _logger.LogInfo($"Attendance for student with id: {creationDto.StudentId} in batch {batchId}");

        await EnsureBatchExists(batchId, batchTrackChanges);

        await EnsureStudentExistsWithRole(creationDto.StudentId);

        await EnsureStudentNotAttendedInBatch(creationDto.StudentId, batchId, attTrackChanges);

        var attendanceEntity = _mapper.Map<Attendance>(creationDto);
        attendanceEntity.MarkedByTeacherId = teacherId;
        attendanceEntity.BatchId = batchId;

        _repository.Attendance.AddAttendanceAsync(attendanceEntity);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Attendance for student with id: {creationDto.StudentId} was created for batch {batchId}.");

        var attendanceDto = _mapper.Map<AttendanceDto>(attendanceEntity);

        return attendanceDto;
    }

    public async Task DeleteAttendanceAsync(Guid batchId, Guid teacherId, Guid id, bool batchTrackChanges, bool attTrackChanges)
    {
        _logger.LogInfo($"Removing attendance with id: {id} in batch {batchId}.");

        await EnsureBatchExists(batchId, batchTrackChanges);

        var attendanceEntity = await _repository.Attendance.GetAttendanceForBatch(batchId, id, attTrackChanges);
        if (attendanceEntity is null)
            throw new AttendanceNotFoundException(id);

        if (attendanceEntity.MarkedByTeacherId != teacherId)
            throw new AttendanceNotForTeacherException(teacherId);

        _repository.Attendance.RemoveAttendanceAsync(attendanceEntity);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Attendance with id: {id} is removed.");
    }

    public async Task<(IEnumerable<AttendanceDto> AttendanceDtos, MetaData MetaData)> GetAttendanceForBatchAsync(Guid batchId, RequestParameters requestParameters, bool batchTrackChanges, bool attTrackChanges)
    {
        _logger.LogInfo($"Retrieving attendance for batch with id {batchId}.");

        await EnsureBatchExists(batchId, batchTrackChanges);

        var attendancesWithMetaData = await _repository.Attendance.GetAllForBatchAsync(batchId, requestParameters, attTrackChanges);

        var attendanedDtos = _mapper.Map<IEnumerable<AttendanceDto>>(attendancesWithMetaData);

        return (attendanedDtos, attendancesWithMetaData.MetaData);
    }

    public async Task<AttendanceDto> GetAttendanceForBatchAsync(Guid batchId, Guid id, bool batchTrackChanges, bool attTrackChanges)
    {
        _logger.LogInfo($"Retrieving attendance with id: {id} for batch {batchId}.");

        await EnsureBatchExists(batchId, batchTrackChanges);

        var attendanceEntity = await _repository.Attendance.GetAttendanceForBatch(batchId, id, attTrackChanges);
        if (attendanceEntity is null)
            throw new AttendanceNotFoundException(id);

        var attendanceDto = _mapper.Map<AttendanceDto>(attendanceEntity);

        _logger.LogInfo($"Retrieved attendance with id: {id} for batch {batchId}.");

        return attendanceDto;
    }

    public async Task<AttendanceDto> GetAttendanceForStudentAsync(Guid batchId, Guid studentId, bool batchTrackChanges, bool attTrackChanges)
    {
        _logger.LogInfo($"Retrieving attendance for student with id: {studentId} for batch {batchId}.");

        await EnsureBatchExists(batchId, batchTrackChanges);

        var attendanceEntity = await _repository.Attendance.GetAttendanceForStudenthAsync(batchId, studentId, attTrackChanges);
        if(attendanceEntity is null)
            throw new AttendanceNotFoundException(studentId);

        var attendanedDto = _mapper.Map<AttendanceDto>(attendanceEntity);

        return attendanedDto;
    }

    public async Task UpdateAttendanceAsync(Guid batchId, Guid teacherId, Guid id, AttendanceForUpdateDto updateDto, bool batchTrackChanges, bool attTrackChanges)
    {
        _logger.LogInfo($"Updating attendance with id: {id} for batch {batchId}.");

        await EnsureBatchExists(batchId, batchTrackChanges);

        var attendanceEntity = await _repository.Attendance.GetAttendanceForBatch(batchId, id, attTrackChanges);
        if (attendanceEntity is null)
            throw new AttendanceNotFoundException(id);

        if (attendanceEntity.MarkedByTeacherId != teacherId)
            throw new AttendanceNotForTeacherException(teacherId);

        _mapper.Map(attendanceEntity, updateDto);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Update attendance with id: {id} succssefully.");
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
        if (user is null)
        {
            _logger.LogWarn($"Student with id: {studentId} not found.");
            throw new UserNotFoundException(studentId);
        }

        var requiredRole = RolesEnum.Student.ToString();
        if (!await _userManager.IsInRoleAsync(user, requiredRole))
        {
            _logger.LogWarn($"User with id: {studentId} is not a Student.");
            throw new UserNotInRoleException(studentId);
        }

        _logger.LogDebug($"Student with id: {studentId} exists.");
    }

    private async Task EnsureStudentNotAttendedInBatch(Guid studentId, Guid batchId, bool trackChanges)
    {
        if (await _repository.Attendance.ExistsAsync(batchId, studentId, trackChanges))
        {
            _logger.LogWarn($"Student with id: {studentId} attended in batch {batchId}.");
            throw new StudentIsEnrolledException(studentId);
        }

        _logger.LogInfo($"Student with id: {studentId} is not enrolled.");
    }
}
