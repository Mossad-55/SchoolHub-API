using System;
using AutoMapper;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.DTOs.Admin;
using SchoolHubAPI.Shared.DTOs.Assignment;
using SchoolHubAPI.Shared.DTOs.Attendance;
using SchoolHubAPI.Shared.DTOs.Batch;
using SchoolHubAPI.Shared.DTOs.Course;
using SchoolHubAPI.Shared.DTOs.Department;
using SchoolHubAPI.Shared.DTOs.Student;
using SchoolHubAPI.Shared.DTOs.StudentBatch;
using SchoolHubAPI.Shared.DTOs.Submission;
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
                opts => opts.MapFrom(x => x.Email));
        CreateMap<UserUpdateDto, User>();

        // Department Mapping
        CreateMap<Department, DepartmentDto>()
            .ForMember(d => d.CreatedDate, 
                opts => opts.MapFrom(src => FormatDate(src.CreatedDate)))
            .ForMember(d => d.UpdatedDate, 
                opts => opts.MapFrom(src => FormatDate(src.UpdatedDate)))
            .ForMember(d => d.HeadOfDepartmentName,
                opts => opts.MapFrom(src =>
                    src.HeadOfDepartment != null && src.HeadOfDepartment.User != null
                        ? src.HeadOfDepartment.User.Name
                        : string.Empty));
        CreateMap<DepartmentForCreationDto, Department>();
        CreateMap<DepartmentForUpdateDto, Department>();

        // Course Mapping
        CreateMap<Course, CourseDto>()
            .ForMember(c => c.CreatedDate,
                opts => opts.MapFrom(src => FormatDate(src.CreatedDate)))
            .ForMember(c => c.UpdatedDate,
                opts => opts.MapFrom(src => FormatDate(src.UpdatedDate)));
        CreateMap<CourseForCreationDto, Course>();
        CreateMap<CourseForUpdateDto, Course>();

        // Batch Mapping
        CreateMap<Batch, BatchDto>()
            .ForMember(b => b.CreatedDate,
                opts => opts.MapFrom(src => FormatDate(src.CreatedDate)))
            .ForMember(b => b.UpdatedDate,
                opts => opts.MapFrom(src => FormatDate(src.UpdatedDate)))
            .ForMember(b => b.StartDate,
                opts => opts.MapFrom(src => FormatDateAndTime(src.StartDate)))
            .ForMember(b => b.EndDate,
                opts => opts.MapFrom(src => FormatDateAndTime(src.EndDate)));
        CreateMap<BatchForCreationDto, Batch>();
        CreateMap<BatchForUpdateDto, Batch>();

        // StudentBatch Mapping
        CreateMap<StudentBatch, StudentBatchDto>()
            .ForMember(sb => sb.BatchName,
                opts => opts.MapFrom(src => 
                    src.Batch != null ? src.Batch.Name : string.Empty))
            .ForMember(sb => sb.StudentName,
                opts => opts.MapFrom(src => 
                    src.Student != null && src.Student.User != null
                    ? src.Student.User.Name : string.Empty))
            .ForMember(sb => sb.EnrollmentDate,
                opts => opts.MapFrom(src => FormatDate(src.EnrollmentDate)));

        CreateMap<StudentBatch, BatchForStudentDto>()
            .ForMember(sb => sb.BatchName,
                opts => opts.MapFrom(src =>
                    src.Batch != null ? src.Batch.Name : string.Empty))
            .ForMember(sb => sb.CourseName,
                opts => opts.MapFrom(src =>
                    src.Batch != null && src.Batch.Course != null ?
                    src.Batch.Course.Name : string.Empty))
            .ForMember(sb => sb.Semester,
                opts => opts.MapFrom(src =>
                    src.Batch != null ? src.Batch.Semester : string.Empty))
            .ForMember(sb => sb.EndDate,
                opts => opts.MapFrom(src =>
                    src.Batch != null ? FormatDateAndTime(src.Batch.EndDate) : string.Empty))
            .ForMember(sb => sb.StartDate,
                opts => opts.MapFrom(src =>
                    src.Batch != null ? FormatDateAndTime(src.Batch.StartDate) : string.Empty))
            .ForMember(sb => sb.IsActive,
                opts => opts.MapFrom(src =>
                    src.Batch != null ? src.Batch.IsActive : false));

        // Attendance Mapping
        CreateMap<Attendance, AttendanceDto>()
            .ForMember(a => a.BatchName,
                opts => opts.MapFrom(src =>
                    src.Batch != null ? src.Batch.Name : string.Empty))
            .ForMember(a => a.StudentName,
                opts => opts.MapFrom(src =>
                    src.Student != null && src.Student.User != null ? src.Student.User.Name : string.Empty))
            .ForMember(a => a.MarkeyByTeacherName,
                opts => opts.MapFrom(src =>
                    src.Teacher != null && src.Teacher.User != null ? src.Teacher.User.Name : string.Empty))
            .ForMember(a => a.Date,
                opts => opts.MapFrom(src => FormatDateAndTime(src.Date)))
            .ForMember(a => a.BatchName,
                opts => opts.MapFrom(src => FormatDate(src.CreatedDate)));
        CreateMap<AttendanceForCreationDto, Attendance>();
        CreateMap<AttendanceForUpdateDto, Attendance>();

        // Assignment Mapping
        CreateMap<Assignment, AssignmentDto>()
            .ForMember(a => a.CreatedDate,
                opts => opts.MapFrom(src => FormatDateAndTime(src.CreatedDate)))
            .ForMember(a => a.DueDate,
                opts => opts.MapFrom(src => FormatDateAndTime(src.DueDate)))
            .ForMember(a => a.CreatedByTeacherName,
                opts => opts.MapFrom(src =>
                    src.Teacher != null && src.Teacher.User != null ? src.Teacher.User.Name : string.Empty))
            .ForMember(a => a.BatchName,
                opts => opts.MapFrom(src =>
                    src.Batch != null ? src.Batch.Name : string.Empty));
        CreateMap<AssignmentForCreationDto, Assignment>();
        CreateMap<AssignmentForUpdateDto, Assignment>();

        // Submission Mapping
        CreateMap<Submission, SubmissionDto>()
            .ForMember(s => s.SubmittedDate,
                opts => opts.MapFrom(src => FormatDateAndTime(src.SubmittedDate)))
            .ForMember(s => s.TeacherName,
                opts => opts.MapFrom(src =>
                    src.Teacher != null && src.Teacher.User != null ? src.Teacher.User.Name : string.Empty))
            .ForMember(s => s.StudentName,
                opts => opts.MapFrom(src =>
                    src.Student != null && src.Student.User != null ? src.Student.User.Name : string.Empty))
            .ForMember(s => s.AssignmentTitle,
                opts => opts.MapFrom(src =>
                    src.Assignment != null ? src.Assignment.Title : string.Empty));
        CreateMap<SubmissionForCreationDto, Submission>();
        CreateMap<SubmissionForUpdateDto, Submission>();
        CreateMap<GradeSubmissionForUpdateDto, Submission>();
    }

    private static string FormatDate(DateTime? dt) =>
        dt.HasValue ? dt.Value.ToString("MMM dd, yyyy") : string.Empty;

    private static string FormatDateAndTime(DateTime? dt) =>
        dt.HasValue ? dt.Value.ToString("dd-MM-yyyy hh:mm tt") : string.Empty;
}
