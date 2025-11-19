using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Entities.Exceptions;
using SchoolHubAPI.Service.Contracts;
using SchoolHubAPI.Shared;
using SchoolHubAPI.Shared.DTOs.Department;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Service;

internal sealed class DepartmentService : IDepartmentService
{
    private readonly IRepositoryManager _repository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _logger;

    public DepartmentService(IRepositoryManager repositoryManager, UserManager<User> userManager, IMapper mapper, ILoggerManager logger)
    {
        _repository = repositoryManager;
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DepartmentDto?> CreateAsync(DepartmentForCreationDto creationDto, bool trackChanges)
    {
        _logger.LogInfo($"Creating department '{creationDto.Name}'");

        // Check if department exists
        if (await _repository.Department.ChechIfDepatmentExists(creationDto.Name!.Trim().ToUpperInvariant(), trackChanges))
        {
            _logger.LogWarn($"Department creation failed: department '{creationDto.Name}' already exists");
            throw new DepartmentExistsException(creationDto.Name);
        }

        // Check if head of department is valid
        if (creationDto.HeadOfDepartmentId.HasValue)
            await CheckIfUserExistsAndRole(creationDto.HeadOfDepartmentId.Value);
        
        var departmentEntity = _mapper.Map<Department>(creationDto);
        _repository.Department.CreateDepartment(departmentEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Department '{departmentEntity.Name}' created (Id: {departmentEntity.Id})");

        return _mapper.Map<DepartmentDto>(departmentEntity);
    }

    public async Task DeleteAsync(Guid id, bool trackChanges)
    {
        _logger.LogInfo($"Attempting to delete department {id}");

        var departmentEntity = await GetDepartment(id, trackChanges);

        _repository.Department.DeleteDepartment(departmentEntity);
        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Department {id} deleted successfully");
    }

    public async Task<(IEnumerable<DepartmentDto> departmentDtos, MetaData MetaData)> GetAllAsync(RequestParameters requestParameters, bool trackChanges)
    {
        _logger.LogDebug($"Fetching all departments (trackChanges={trackChanges}, page={requestParameters.PageNumber}, size={requestParameters.PageSize}, search='{requestParameters.SearchTerm}', orderBy='{requestParameters.OrderBy}')");

        var departmentEntitiesWithMetaData = await _repository.Department.GetAllDepartmentsAsync(requestParameters, trackChanges)!;

        var departmentDtos = _mapper.Map<IEnumerable<DepartmentDto>>(departmentEntitiesWithMetaData);

        _logger.LogInfo($"Fetched {departmentDtos?.Count() ?? 0} departments (page {departmentEntitiesWithMetaData.MetaData.CurrentPage} of {departmentEntitiesWithMetaData.MetaData.TotalPages})");

        return (departmentDtos, departmentEntitiesWithMetaData.MetaData)!;
    }

    public async Task<DepartmentDto?> GetByIdAsync(Guid id, bool trackChanges)
    {
        _logger.LogDebug($"Fetching department by id {id} (trackChanges={trackChanges})");

        var departmentEntity = await GetDepartment(id, trackChanges);

        var departmentDto = _mapper.Map<DepartmentDto>(departmentEntity);

        _logger.LogInfo($"Fetched department {departmentDto?.Id} - '{departmentDto?.Name}'");

        return departmentDto;
    }

    public async Task UpdateAsync(Guid id, DepartmentForUpdateDto updateDto, bool trackChanges)
    {
        _logger.LogInfo($"Updating department {id}");

        var departmentEntity = await GetDepartment(id, trackChanges);

        // Check if department exists
        if (await _repository.Department.ChechIfDepatmentExists(updateDto.Name!.Trim().ToUpperInvariant(), trackChanges) && updateDto.Name != departmentEntity.Name)
        {
            _logger.LogWarn($"Department update failed: department '{updateDto.Name}' already exists");
            throw new DepartmentExistsException(updateDto.Name);
        }

        // Check if head of department is valid
        if (updateDto.HeadOfDepartmentId.HasValue)
            await CheckIfUserExistsAndRole(updateDto.HeadOfDepartmentId.Value);

        _mapper.Map(updateDto, departmentEntity);

        await _repository.SaveChangesAsync();

        _logger.LogInfo($"Department {id} updated successfully");
    }


    // Private Functions.
    private async Task CheckIfUserExistsAndRole(Guid id)
    {
        _logger.LogDebug($"Validating head of department user {id}");

        // Check if exists
        var headUser = await _userManager.FindByIdAsync(id.ToString());
        if (headUser is null)
        {
            _logger.LogWarn($"Head of department validation failed: user not found ({id})");
            throw new UserNotFoundException(id);
        }

        // Check Role
        var requiredRole = RolesEnum.Teacher.ToString();
        if (!await _userManager.IsInRoleAsync(headUser, requiredRole))
        {
            _logger.LogWarn($"Head of department validation failed: user {id} is not in role '{requiredRole}'");
            throw new HeadOfDepartmentException();
        }

        _logger.LogDebug($"Head of department validated for user {id}");
    }

    private async Task<Department> GetDepartment(Guid id, bool trackChanges)
    {
        var departmentEntity = await _repository.Department.GetDepartmentAsync(id, trackChanges);
        if (departmentEntity is null)
        {
            _logger.LogWarn($"Retrieve failed. Department not found: {id}");
            throw new DepartmentNotFoundException(id);
        }

        return departmentEntity;
    }
}
