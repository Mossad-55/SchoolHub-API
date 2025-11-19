using AutoMapper;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.DTOs.Admin;
using SchoolHubAPI.Shared.DTOs.Student;
using SchoolHubAPI.Shared.DTOs.Teacher;
using SchoolHubAPI.Shared.DTOs.User;

namespace SchoolHubAPI;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Admin Mapping
        CreateMap<User, AdminDto>()
            .ForMember(a => a.CreatedDate,
                opts => opts.MapFrom(x => FormatDate(x.CreatedDate)))
            .ForMember(a => a.UpdatedDate,
                opts => opts.MapFrom(x => FormatDate(x.UpdatedDate)))
            .ForMember(a => a.Name, opts => opts.MapFrom(x => x.Name))
            .ForMember(a => a.Email, opts => opts.MapFrom(x => x.Email));

        CreateMap<Admin, AdminDto>()
            .IncludeMembers(a => a.User)
            .ForMember(d => d.UserId, opts => opts.MapFrom(s => s.UserId));

        // Teacher Mapping
        CreateMap<User, TeacherDto>()
            .ForMember(a => a.CreatedDate,
                opts => opts.MapFrom(x => FormatDate(x.CreatedDate)))
            .ForMember(a => a.UpdatedDate,
                opts => opts.MapFrom(x => FormatDate(x.UpdatedDate)))
            .ForMember(a => a.Name, opts => opts.MapFrom(x => x.Name))
            .ForMember(a => a.Email, opts => opts.MapFrom(x => x.Email));

        CreateMap<Teacher, TeacherDto>()
            .IncludeMembers(t => t.User)
            .ForMember(d => d.UserId, opts => opts.MapFrom(s => s.UserId));

        // Student Mapping 
        CreateMap<User, StudentDto>()
            .ForMember(a => a.CreatedDate,
                opts => opts.MapFrom(x => FormatDate(x.CreatedDate)))
            .ForMember(a => a.UpdatedDate,
                opts => opts.MapFrom(x => FormatDate(x.UpdatedDate)))
            .ForMember(a => a.Name, opts => opts.MapFrom(x => x.Name))
            .ForMember(a => a.Email, opts => opts.MapFrom(x => x.Email));

        CreateMap<Student, StudentDto>()
            .IncludeMembers(s => s.User)
            .ForMember(d => d.UserId, opts => opts.MapFrom(src => src.UserId));

        // User Mapping for registration/update
        CreateMap<UserRegisterationDto, User>()
            .ForMember(u => u.UserName,
                opts => opts.MapFrom(x => x.Name!.Replace(" ", "")));
        CreateMap<UserUpdateDto, User>()
            .ForMember(u => u.UserName,
                opts => opts.MapFrom(x => x.Name!.Replace(" ", "")));
    }

    private static string FormatDate(DateTime? dt) =>
        dt.HasValue ? dt.Value.ToString("MMM dd, yyyy") : string.Empty;
}