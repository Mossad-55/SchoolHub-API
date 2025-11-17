using AutoMapper;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.DTOs.Admin;
using SchoolHubAPI.Shared.DTOs.Student;
using SchoolHubAPI.Shared.DTOs.Teacher;

namespace SchoolHubAPI;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Admin Mapping
        CreateMap<User, AdminDto>()
            .ForMember(a => a.CreatedDate,
            opts => opts.MapFrom(x => x.CreatedDate.HasValue ? x.CreatedDate.Value.ToString("MMM dd, yyyy") : string.Empty))
            .ForMember(a => a.UpdatedDate,
            opts => opts.MapFrom(x => x.UpdatedDate.HasValue ? x.UpdatedDate.Value.ToString("MMM dd, yyyy") : string.Empty));

        // Teacher Mapping
        CreateMap<User, TeacherDto>()
            .ForMember(a => a.CreatedDate,
            opts => opts.MapFrom(x => x.CreatedDate.HasValue ? x.CreatedDate.Value.ToString("MMM dd, yyyy") : string.Empty))
            .ForMember(a => a.UpdatedDate,
            opts => opts.MapFrom(x => x.UpdatedDate.HasValue ? x.UpdatedDate.Value.ToString("MMM dd, yyyy") : string.Empty));

        // Student Mapping
        CreateMap<User, StudentDto>()
            .ForMember(a => a.CreatedDate,
            opts => opts.MapFrom(x => x.CreatedDate.HasValue ? x.CreatedDate.Value.ToString("MMM dd, yyyy") : string.Empty))
            .ForMember(a => a.UpdatedDate,
            opts => opts.MapFrom(x => x.UpdatedDate.HasValue ? x.UpdatedDate.Value.ToString("MMM dd, yyyy") : string.Empty));
    }
}
