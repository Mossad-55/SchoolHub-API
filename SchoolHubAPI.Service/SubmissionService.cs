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
        _logger.LogDebug($"Deleting submission {id} for assignment {assignmentId} by student {studentId}");

        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);
        await EnsureStudentExistsWithRole(studentId);

        var submissionEntity = await GetSubmissionForAssignment(assignmentId, id, subTrackChanges);
        if (submissionEntity.StudentId != studentId)
        {
            _logger.LogWarn($"Submission {id} is not owned by student {studentId}");
            throw new SubmissionNotOwnedByStudentException(studentId);
        }

        _repository.Submission.DeleteSubmission(submissionEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Submission {id} deleted successfully for assignment {assignmentId}");

        // Notify teacher
        var assignment = await _repository.Assignment.GetByIdAsync(assignmentId, assignmentTrackChanges);
        if (assignment?.CreatedByTeacherId != null)
        {
            var teacherNotification = new Notification
            {
                Title = "Submission Deleted",
                Message = $"Student '{studentId}' deleted submission '{id}' for assignment '{assignmentId}'.",
                RecipientRole = RecipientRole.Teacher,
                RecipientId = assignment.CreatedByTeacherId,
                CreatedDate = DateTime.UtcNow
            };
            _repository.Notification.AddNotification(teacherNotification);
            await _repository.SaveChangesAsync();
        }
    }

    public async Task<(IEnumerable<SubmissionDto> SubmissionDtos, MetaData MetaData)> GetAllForAssignmentAsync(Guid assignmentId, RequestParameters requestParameters, bool assignmentTrackChanges, bool subTrackChanges)
    {
        _logger.LogDebug($"Fetching all submissions for assignment {assignmentId}");

        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);

        var submissionWithMetaData = await _repository.Submission.GetAllForAssignmentAsync(assignmentId, requestParameters, subTrackChanges);
        var submissionDtos = _mapper.Map<IEnumerable<SubmissionDto>>(submissionWithMetaData);

        _logger.LogInfo($"Fetched {submissionDtos.Count()} submissions for assignment {assignmentId}");

        return (submissionDtos, submissionWithMetaData.MetaData);
    }

    public async Task<SubmissionDto?> GetByIdAsync(Guid assignmentId, Guid id, bool assignmentTrackChanges, bool subTrackChanges)
    {
        _logger.LogDebug($"Fetching submission {id} for assignment {assignmentId}");

        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);

        var submissionEntity = await GetSubmissionForAssignment(assignmentId, id, subTrackChanges);
        var submissionDto = _mapper.Map<SubmissionDto>(submissionEntity);

        _logger.LogInfo($"Submission {id} retrieved successfully for assignment {assignmentId}");

        return submissionDto;
    }

    public async Task<SubmissionDto?> SubmitAsync(Guid assignmentId, Guid studentId, SubmissionForCreationDto creationDto, bool assignmentTrackChanges, bool subTrackChanges)
    {
        _logger.LogDebug($"Student {studentId} submitting assignment {assignmentId}");

        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);
        await EnsureStudentExistsWithRole(studentId);

        if (await _repository.Submission.CheckForSubmissionAsync(assignmentId, studentId, subTrackChanges))
        {
            _logger.LogWarn($"Student {studentId} has already submitted assignment {assignmentId}");
            throw new StudentSubmittedAssignmentException(assignmentId, studentId);
        }

        var submissionEntity = _mapper.Map<Submission>(creationDto);
        submissionEntity.StudentId = studentId;

        _repository.Submission.CreateSubmission(submissionEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Student {studentId} submitted assignment {assignmentId} successfully");

        // --- Notification to Teacher(s) of the assignment ---
        var assignment = await _repository.Assignment.GetByIdAsync(assignmentId, assignmentTrackChanges);
        if (assignment?.CreatedByTeacherId != null)
        {
            var teacherNotification = new Notification
            {
                Title = "New Submission",
                Message = $"Student '{studentId}' submitted assignment '{assignmentId}'.",
                RecipientRole = RecipientRole.Teacher,
                RecipientId = assignment.CreatedByTeacherId,
                CreatedDate = DateTime.UtcNow
            };
            _repository.Notification.AddNotification(teacherNotification);
            await _repository.SaveChangesAsync();
        }
        // --------------------------------------------

        return _mapper.Map<SubmissionDto>(submissionEntity);
    }

    public async Task GradeAsync(Guid assignmentId, Guid teacherId, Guid id, GradeSubmissionForUpdateDto gradeDto, bool assignmentTrackChanges, bool subTrackChanges)
    {
        _logger.LogDebug($"Teacher {teacherId} grading submission {id} for assignment {assignmentId}");

        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);
        await EnsureTeacherExistsWithRole(teacherId);

        var submissionEntity = await GetSubmissionForAssignment(assignmentId, id, subTrackChanges);
        submissionEntity.GradedByTeacherId = teacherId;

        _mapper.Map(gradeDto, submissionEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Submission {id} graded by teacher {teacherId} for assignment {assignmentId}");

        // --- Notification to Student ---
        var studentNotification = new Notification
        {
            Title = "Assignment Graded",
            Message = $"Your submission '{id}' for assignment '{assignmentId}' has been graded by teacher '{teacherId}'.",
            RecipientRole = RecipientRole.Student,
            RecipientId = submissionEntity.StudentId,
            CreatedDate = DateTime.UtcNow
        };
        _repository.Notification.AddNotification(studentNotification);
        await _repository.SaveChangesAsync();
        // --------------------------------------------
    }

    public async Task UpdateAsync(Guid assignmentId, Guid studentId, Guid id, SubmissionForUpdateDto updateDto, bool assignmentTrackChanges, bool subTrackChanges)
    {
        _logger.LogDebug($"Updating submission {id} for assignment {assignmentId} by student {studentId}");

        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);
        await EnsureStudentExistsWithRole(studentId);

        var submissionEntity = await GetSubmissionForAssignment(assignmentId, id, subTrackChanges);
        if (submissionEntity.StudentId != studentId)
        {
            _logger.LogWarn($"Submission {id} is not owned by student {studentId}");
            throw new SubmissionNotOwnedByStudentException(studentId);
        }

        _mapper.Map(updateDto, submissionEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Submission {id} updated successfully for assignment {assignmentId}");

        // Notify teacher
        var assignment = await _repository.Assignment.GetByIdAsync(assignmentId, assignmentTrackChanges);
        if (assignment?.CreatedByTeacherId != null)
        {
            var teacherNotification = new Notification
            {
                Title = "Submission Updated",
                Message = $"Student '{studentId}' updated submission '{id}' for assignment '{assignmentId}'.",
                RecipientRole = RecipientRole.Teacher,
                RecipientId = assignment.CreatedByTeacherId,
                CreatedDate = DateTime.UtcNow
            };
            _repository.Notification.AddNotification(teacherNotification);
            await _repository.SaveChangesAsync();
        }
    }

    public async Task<bool> CheckForSubmissionAsync(Guid assignmentId, Guid studentId, bool assignmentTrackChanges, bool subTrackChanges)
    {
        await EnsureAssignmentExistsAsync(assignmentId, assignmentTrackChanges);

        return await _repository.Submission.CheckForSubmissionAsync(assignmentId, studentId, subTrackChanges);
    }

    // Private Functions
    private async Task EnsureAssignmentExistsAsync(Guid assignmentId, bool trackChanges)
    {
        var assignment = await _repository.Assignment.GetByIdAsync(assignmentId, trackChanges);
        if (assignment is null)
        {
            _logger.LogWarn($"Assignment {assignmentId} not found");
            throw new AssignmentNotFoundException(assignmentId);
        }

        _logger.LogDebug($"Assignment {assignmentId} exists");
    }

    private async Task<Submission> GetSubmissionForAssignment(Guid assignmentId, Guid id, bool trackChanges)
    {
        var submissionEntity = await _repository.Submission.GetForAssignmenthByIdAsync(assignmentId, id, trackChanges);
        if (submissionEntity is null)
        {
            _logger.LogWarn($"Submission {id} in assignment {assignmentId} not found");
            throw new SubmissionNotFoundException(id);
        }

        _logger.LogDebug($"Submission {id} retrieved for assignment {assignmentId}");
        return submissionEntity;
    }

    private async Task EnsureTeacherExistsWithRole(Guid teacherId)
    {
        var teacher = await _userManager.FindByIdAsync(teacherId.ToString());
        if (teacher is null)
        {
            _logger.LogWarn($"Teacher {teacherId} not found");
            throw new UserNotFoundException(teacherId);
        }

        if (!await _userManager.IsInRoleAsync(teacher, RolesEnum.Teacher.ToString()))
        {
            _logger.LogWarn($"User {teacherId} is not in role Teacher");
            throw new UserNotInRoleException(teacherId);
        }

        _logger.LogDebug($"Teacher {teacherId} exists and has Teacher role");
    }

    private async Task EnsureStudentExistsWithRole(Guid studentId)
    {
        var student = await _userManager.FindByIdAsync(studentId.ToString());
        if (student is null)
        {
            _logger.LogWarn($"Student {studentId} not found");
            throw new UserNotFoundException(studentId);
        }

        if (!await _userManager.IsInRoleAsync(student, RolesEnum.Student.ToString()))
        {
            _logger.LogWarn($"User {studentId} is not in role Student");
            throw new UserNotInRoleException(studentId);
        }

        _logger.LogDebug($"Student {studentId} exists and has Student role");
    }
}
