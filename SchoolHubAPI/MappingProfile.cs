using AutoMapper;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.DTOs;

namespace SchoolHubAPI;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Admin Mapping
        CreateMap<User, AdminDto>();
    }
}
