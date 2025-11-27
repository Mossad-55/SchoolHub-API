using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.DTOs.Submission;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class SubmissionService : ISubmissionService
{
    private readonly IRepositoryManager _repository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _logger;

    public SubmissionService(IRepositoryManager repository, UserManager<User> userManager, IMapper mapper, ILoggerManager loggerManager)
    {
        _repository = repository;
        _userManager = userManager;
        _mapper = mapper;
        _logger = loggerManager;
    }

    public async Task DeleteAsync(Guid assignmentId, Guid studentId, Guid id, bool assignmentTrackChanges, bool subTrackChanges)
    {
        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);

        await EnsureStudentExistsWithRole(studentId);

        var submissionEntity = await GetSubmissionForAssignment(assignmentId, id, subTrackChanges);
        if (submissionEntity.StudentId != studentId)
            throw new SubmissionNotOwnedByStudentException(studentId);

        _repository.Submission.DeleteSubmission(submissionEntity);

        await _repository.SaveChangesAsync();
    }

    public async Task<(IEnumerable<SubmissionDto> SubmissionDtos, MetaData MetaData)> GetAllForAssignmentAsync(Guid assignmentId, RequestParameters requestParameters, bool assignmentTrackChanges, bool subTrackChanges)
    {
        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);

        var submissionWithMetaData = await _repository.Submission.GetAllForAssignmentAsync(assignmentId, requestParameters, subTrackChanges);

        var submissionDtos = _mapper.Map<IEnumerable<SubmissionDto>>(submissionWithMetaData);

        return (submissionDtos, submissionWithMetaData.MetaData);
    }

    public async Task<SubmissionDto?> GetByIdAsync(Guid assignmentId, Guid id, bool assignmentTrackChanges, bool subTrackChanges)
    {
        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);

        var submissionEntity = await GetSubmissionForAssignment(assignmentId, id, subTrackChanges);

        var submissionDto = _mapper.Map<SubmissionDto>(submissionEntity);

        return submissionDto;
    }

    public async Task GradeAsync(Guid assignmentId, Guid teacherId, Guid id, GradeSubmissionForUpdateDto gradeDto, bool assignmentTrackChanges, bool subTrackChanges)
    {
        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);

        await EnsureTeacherExistsWithRole(teacherId);

        var submissionEntity = await GetSubmissionForAssignment(assignmentId, id, subTrackChanges);
        submissionEntity.GradedByTeacherId = teacherId;

        _mapper.Map(gradeDto, submissionEntity);

        await _repository.SaveChangesAsync();
    }

    public async Task<SubmissionDto?> SubmitAsync(Guid assignmentId, Guid studentId, SubmissionForCreationDto creationDto, bool assignmentTrackChanges, bool subTrackChanges)
    {
        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);

        await EnsureStudentExistsWithRole(studentId);

        if (await _repository.Submission.CheckForSubmissionAsync(assignmentId, studentId, subTrackChanges))
            throw new StudentSubmittedAssignmentException(assignmentId, studentId);

        var submissionEntity = _mapper.Map<Submission>(creationDto);
        submissionEntity.StudentId = studentId;

        _repository.Submission.CreateSubmission(submissionEntity);

        await _repository.SaveChangesAsync();

        return _mapper.Map<SubmissionDto>(submissionEntity);
    }


    public async Task UpdateAsync(Guid assignmentId, Guid studentId, Guid id, SubmissionForUpdateDto updateDto, bool assignmentTrackChanges, bool subTrackChanges)
    {
        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);

        await EnsureStudentExistsWithRole(studentId);

        var submissionEntity = await GetSubmissionForAssignment(assignmentId, id, subTrackChanges);
        if (submissionEntity.StudentId != studentId)
            throw new SubmissionNotOwnedByStudentException(studentId);

        _mapper.Map(updateDto, submissionEntity);

        await _repository.SaveChangesAsync();
    }

    // Private Functions
    private async Task EnsureAssignmentExistsAsync(Guid assignmentId, bool trackChanges)
    {
        var assignment = await _repository.Assignment.GetByIdAsync(assignmentId, trackChanges);
        if (assignment is null)
        {
            _logger.LogWarn($"Assignment with id: {assignmentId} not found.");
            throw new AssignmentNotFoundException(assignmentId);
        }

        _logger.LogDebug($"Assignment with id: {assignmentId} exists.");
    }

    private async Task<Submission> GetSubmissionForAssignment(Guid assignmentId, Guid id, bool trackChanges)
    {
        var submissionEntity = await _repository.Submission.GetForAssignmenthByIdAsync(assignmentId, id, trackChanges);
        if (submissionEntity is null)
        {
            _logger.LogWarn($"Submission with id: {id} in Assignment {assignmentId} not found.");
            throw new SubmissionNotFoundException(id);
        }

        _logger.LogDebug($"Submission with id: {id} retrieved for assignment {assignmentId}.");
        return submissionEntity;
    }

    private async Task EnsureTeacherExistsWithRole(Guid teacherId)
    {
        var teacher = await _userManager.FindByIdAsync(teacherId.ToString());
        if (teacher is null)
            throw new UserNotFoundException(teacherId);

        if (await _userManager.IsInRoleAsync(teacher, RolesEnum.Teacher.ToString()))
            throw new UserNotInRoleException(teacherId);
    }

    private async Task EnsureStudentExistsWithRole(Guid studentId)
    {
        var student = await _userManager.FindByIdAsync(studentId.ToString());
        if (student is null)
            throw new UserNotFoundException(studentId);

        if (await _userManager.IsInRoleAsync(student, RolesEnum.Student.ToString()))
            throw new UserNotInRoleException(studentId);
    }
}
