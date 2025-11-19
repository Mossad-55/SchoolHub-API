using AutoMapper;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared.DTOs.Admin;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class AdminService : IAdminService
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _logger;
    
    public AdminService(IRepositoryManager repository, IMapper mapper, ILoggerManager logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task CreateAsync(Guid userId, bool trackChanges)
    {
        _logger.LogInfo($"Creating admin for user {userId}");

        var admin = new Admin
        {
            UserId = userId
        };

        _repository.Admin.CreateAdminAsync(admin);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Admin created for user {userId}");
    }

    public async Task<(IEnumerable<AdminDto> AdminDtos, MetaData MetaData)> GetAllAsync(RequestParameters requestParameters, bool trackChanges)
    {
        _logger.LogDebug($"Fetching all admins (trackChanges={trackChanges})");

        var adminEntitiesWithMetaData = await _repository.Admin.GetAllAdminsAsync(requestParameters, trackChanges)!;

        var adminDtos = _mapper.Map<IEnumerable<AdminDto>>(adminEntitiesWithMetaData);

        _logger.LogInfo($"Fetched {adminDtos?.Count() ?? 0} admins");

        return (adminDtos, adminEntitiesWithMetaData.MetaData)!;
    }

    public async Task<AdminDto?> GetByIdAsync(Guid id, bool trackChanges)
    {
        _logger.LogDebug($"Fetching admin by id {id} (trackChanges={trackChanges})");

        var adminEntity = await _repository.Admin.GetAdminAsync(id, trackChanges);
        if (adminEntity is null)
        {
            _logger.LogWarn($"Admin not found: {id}");
            throw new UserNotFoundException(id);
        }

        var adminDto = _mapper.Map<AdminDto>(adminEntity);

        _logger.LogInfo($"Fetched admin for user {adminDto?.UserId}");

        return adminDto;
    }
}
