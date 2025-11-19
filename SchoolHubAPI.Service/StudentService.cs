using AutoMapper;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Student;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class StudentService : IStudentService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public StudentService(IRepositoryManager repository, IMapper mapper, ILoggerManager logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task CreateAsync(Guid userId, bool trackChanges)
    {
        _logger.LogInfo($"Creating student for user {userId}");

        var student = new Student
        {
            UserId = userId
        };

        _repository.Student.CreateStudentAsync(student);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Student created for user {userId}");
    }

    public async Task<(IEnumerable<StudentDto> StudentDtos, MetaData MetaData)> GetAllAsync(RequestParameters requestParameters, bool trackChanges)
    {
        _logger.LogDebug($"Fetching all students (trackChanges={trackChanges})");

        var studentEntitiesWithMetaData = await _repository.Student.GetAllStudentsAsync(requestParameters, trackChanges)!;

        var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(studentEntitiesWithMetaData);

        _logger.LogInfo($"Fetched {studentDtos?.Count() ?? 0} students");

        return (studentDtos, studentEntitiesWithMetaData.MetaData)!;
    }

    public async Task<StudentDto?> GetByIdAsync(Guid id, bool trackChanges)
    {
        _logger.LogDebug($"Fetching student by id {id} (trackChanges={trackChanges})");

        var studentEntity = await _repository.Student.GetStudentAsync(id, trackChanges);
        if (studentEntity is null)
        {
            _logger.LogWarn($"Student not found: {id}");
            throw new UserNotFoundException(id);
        }

        var studentDto = _mapper.Map<StudentDto>(studentEntity);

        _logger.LogInfo($"Fetched student for user {studentDto?.UserId}");

        return studentDto;
        throw new NotImplementedException();
    }
}
