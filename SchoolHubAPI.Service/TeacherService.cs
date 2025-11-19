using AutoMapper;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Teacher;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class TeacherService : ITeacherService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public TeacherService(IRepositoryManager repository, IMapper mapper, ILoggerManager logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task CreateAsync(Guid userId, bool trackChanges)
    {
        _logger.LogInfo($"Creating teacher for user {userId}");

        var teacher = new Teacher
        {
            UserId = userId
        };

        _repository.Teacher.CreateTeacherAsync(teacher);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Teacher created for user {userId}");
    }

    public async Task<(IEnumerable<TeacherDto> TeacherDtos, MetaData MetaData)> GetAllAsync(RequestParameters requestParameters, bool trackChanges)
    {
        _logger.LogDebug($"Fetching all teachers (trackChanges={trackChanges})");

        var teacherEntitiesWithMetaData = await _repository.Teacher.GetAllTeachersAsync(requestParameters, trackChanges)!;

        var teacherDtos = _mapper.Map<IEnumerable<TeacherDto>>(teacherEntitiesWithMetaData);

        _logger.LogInfo($"Fetched {teacherDtos?.Count() ?? 0} teachers");

        return (teacherDtos, teacherEntitiesWithMetaData.MetaData)!;
    }

    public async Task<TeacherDto?> GetByIdAsync(Guid id, bool trackChanges)
    {
        _logger.LogDebug($"Fetching teacher by id {id} (trackChanges={trackChanges})");

        var teacherEntity = await _repository.Teacher.GetTeacherAsync(id, trackChanges);
        if (teacherEntity is null)
        {
            _logger.LogWarn($"Teacher not found: {id}");
            throw new UserNotFoundException(id);
        }

        var teacherDto = _mapper.Map<TeacherDto>(teacherEntity);

        _logger.LogInfo($"Fetched teacher for user {teacherDto?.UserId}");

        return teacherDto;
    }
}
