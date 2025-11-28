using AutoMapper;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.DTOs.Course;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class CourseService : ICourseService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _logger;

    public CourseService(IRepositoryManager repository, IMapper mapper, ILoggerManager logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CourseDto?> CreateAsync(Guid departmentId, CourseForCreationDto creationDto, bool depTrackChanges, bool courseTrackChanges)
    {
        _logger.LogInfo($"Creating course in department {departmentId}.");

        await EnsureDepartmentExistsAsync(departmentId, depTrackChanges);

        var normalizedCode = creationDto.Code?.Trim().ToUpperInvariant() ?? string.Empty;
        await EnsureCourseCodeIsUniqueAsync(departmentId, normalizedCode, courseTrackChanges);

        var courseEntity = _mapper.Map<Course>(creationDto);
        courseEntity.DepartmentId = departmentId;

        _repository.Course.CreateCourse(courseEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Course created with id: {courseEntity.Id} in department {courseEntity.DepartmentId}.");

        // --- Notifications ---
        var department = await _repository.Department.GetDepartmentAsync(departmentId, false);

        // Notify Admins
        var adminNotification = new Notification
        {
            Title = "Course Created",
            Message = $"New course '{courseEntity.Name}' created in department '{departmentId}'.",
            RecipientRole = RecipientRole.Admin,
            CreatedDate = DateTime.UtcNow
        };
        _repository.Notification.AddNotification(adminNotification);

        // Notify Head of Department if assigned
        if (department?.HeadOfDepartmentId != null)
        {
            var hodNotification = new Notification
            {
                Title = "Course Created",
                Message = $"New course '{courseEntity.Name}' created in your department '{department.Name}'.",
                RecipientRole = RecipientRole.Teacher,
                RecipientId = department.HeadOfDepartmentId.Value,
                CreatedDate = DateTime.UtcNow
            };
            _repository.Notification.AddNotification(hodNotification);
        }

        await _repository.SaveChangesAsync();
        // -------------------

        return _mapper.Map<CourseDto>(courseEntity);
    }

    public async Task UpdateAsync(Guid departmentId, Guid id, CourseForUpdateDto updateDto, bool depTrackChanges, bool courseTrackChanges)
    {
        _logger.LogInfo($"Updating course {id} in department {departmentId}.");

        await EnsureDepartmentExistsAsync(departmentId, depTrackChanges);

        var courseEntity = await GetCourseForDepartment(departmentId, id, courseTrackChanges);

        var normalizedCode = updateDto.Code?.Trim().ToUpperInvariant()!;
        await EnsureCourseCodeIsUniqueAsync(departmentId, normalizedCode, courseTrackChanges, currentCode: courseEntity.Code);

        _mapper.Map(updateDto, courseEntity);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Course {id} updated in department {departmentId}.");

        // --- Notifications ---
        var department = await _repository.Department.GetDepartmentAsync(departmentId, false);

        var adminNotification = new Notification
        {
            Title = "Course Updated",
            Message = $"Course '{courseEntity.Name}' updated in department '{departmentId}'.",
            RecipientRole = RecipientRole.Admin,
            CreatedDate = DateTime.UtcNow
        };
        _repository.Notification.AddNotification(adminNotification);

        if (department?.HeadOfDepartmentId != null)
        {
            var hodNotification = new Notification
            {
                Title = "Course Updated",
                Message = $"Course '{courseEntity.Name}' updated in your department '{department.Name}'.",
                RecipientRole = RecipientRole.Teacher,
                RecipientId = department.HeadOfDepartmentId.Value,
                CreatedDate = DateTime.UtcNow
            };
            _repository.Notification.AddNotification(hodNotification);
        }

        await _repository.SaveChangesAsync();
        // -------------------
    }

    public async Task DeleteAsync(Guid departmentId, Guid id, bool depTrackChanges, bool courseTrackChanges)
    {
        _logger.LogInfo($"Soft-deleting course {id} from department {departmentId}.");

        // Ensure department exists
        await EnsureDepartmentExistsAsync(departmentId, depTrackChanges);

        // Get course entity
        var courseEntity = await GetCourseForDepartment(departmentId, id, courseTrackChanges);

        if (!courseEntity.IsActive)
        {
            _logger.LogWarn($"Course {id} is already inactive.");

            throw new CourseIsAlreadyInActiveException(id);
        }

        // Soft delete
        courseEntity.IsActive = false;
        courseEntity.UpdatedDate = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        _logger.LogInfo($"Course {id} soft-deleted from department {departmentId}.");

        // --- Notifications ---
        var department = await _repository.Department.GetDepartmentAsync(departmentId, false);

        var adminNotification = new Notification
        {
            Title = "Course Deleted",
            Message = $"Course '{courseEntity.Name}' was deleted from department '{departmentId}'.",
            RecipientRole = RecipientRole.Admin,
            CreatedDate = DateTime.UtcNow
        };
        _repository.Notification.AddNotification(adminNotification);

        if (department?.HeadOfDepartmentId != null)
        {
            var hodNotification = new Notification
            {
                Title = "Course Deleted",
                Message = $"Course '{courseEntity.Name}' deleted from your department '{department.Name}'.",
                RecipientRole = RecipientRole.Teacher,
                RecipientId = department.HeadOfDepartmentId.Value,
                CreatedDate = DateTime.UtcNow
            };
            _repository.Notification.AddNotification(hodNotification);
        }

        await _repository.SaveChangesAsync();
        // -------------------
    }


    public async Task<(IEnumerable<CourseDto> CourseDtos, MetaData MetaData)> GetAllAsync(Guid departmentId, RequestParameters requestParameters, bool depTrackChanges, bool courseTrackChanges)
    {
        _logger.LogInfo($"Retrieving courses for department {departmentId} - page {requestParameters.PageNumber}, size {requestParameters.PageSize}.");

        await EnsureDepartmentExistsAsync(departmentId, depTrackChanges);

        var courseEntitiesWithMetaData = await _repository.Course.GetAllCoursesAsync(departmentId, requestParameters, courseTrackChanges)!;

        var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courseEntitiesWithMetaData);

        return (courseDtos, courseEntitiesWithMetaData.MetaData)!;
    }

    public async Task<CourseDto?> GetByIdForDepartmentAsync(Guid departmentId, Guid id, bool depTrackChanges, bool courseTrackChanges)
    {
        _logger.LogInfo($"Retrieving course {id} from department {departmentId}.");

        await EnsureDepartmentExistsAsync(departmentId, courseTrackChanges);

        var courseEntity = await GetCourseForDepartment(departmentId, id, courseTrackChanges);

        var courseDto = _mapper.Map<CourseDto>(courseEntity);

        _logger.LogDebug($"Course {id} mapped to DTO for department {departmentId}.");

        return courseDto;
    }

    // Private Functions
    private async Task EnsureDepartmentExistsAsync(Guid departmentId, bool trackChanges)
    {
        var department = await _repository.Department.GetDepartmentAsync(departmentId, trackChanges);
        if (department is null)
        {
            _logger.LogWarn($"Department with id: {departmentId} not found.");
            throw new DepartmentNotFoundException(departmentId);
        }

        _logger.LogDebug($"Department with id: {departmentId} exists.");
    }

    private async Task<Course> GetCourseForDepartment(Guid departmentId, Guid id, bool trackChanges)
    {
        var courseEntity = await _repository.Course.GetCourseForDepartmentAsync(departmentId, id, trackChanges);
        if (courseEntity is null)
        {
            _logger.LogWarn($"Course with id: {id} in department {departmentId} not found.");
            throw new CourseNotFoundException(id);
        }

        _logger.LogDebug($"Course with id: {id} retrieved for department {departmentId}.");
        return courseEntity;
    }

    private async Task EnsureCourseCodeIsUniqueAsync(Guid departmentId, string code, bool trackChanges, string? currentCode = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;

        if (await _repository.Course.CheckIfCourseExists(departmentId, code, trackChanges)
            && !string.Equals(code, currentCode, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarn($"Course with code '{code}' already exists in department {departmentId}.");
            throw new CourseExistsException(code);
        }

        _logger.LogDebug($"Course code '{code}' is unique in department {departmentId} or unchanged.");
    }
}
