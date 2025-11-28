using Microsoft.EntityFrameworkCore;
using SchoolHubAPI.Contracts;
using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Repository.Extensions;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Repository;

internal sealed class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    public CourseRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }

    public async Task<bool> CheckIfCourseExists(Guid departmentId, string code, bool trackChanges) =>
        await FindByCondition(c => c.DepartmentId == departmentId && c.Code == code, trackChanges)
        .SingleOrDefaultAsync() != null;

    public void CreateCourse(Course course) => Create(course);

    public void DeleteCourse(Course course) => Delete(course);

    public async Task<PagedList<Course>>? GetAllCoursesAsync(Guid departmentId, RequestParameters requestParameters, bool trackChanges)
    {
        var courses = await FindByCondition(c => c.DepartmentId == departmentId && c.IsActive == requestParameters.IsActive , trackChanges)
            .Search(requestParameters.SearchTerm!)
            .Sort(requestParameters.OrderBy!)
            .ToListAsync();

        return PagedList<Course>.ToPagedList(courses, requestParameters.PageNumber, requestParameters.PageSize);
    }

    public async Task<Course?> GetCourseForDepartmentAsync(Guid departmentId, Guid id, bool trackChanges) =>
        await FindByCondition(c => c.DepartmentId == departmentId && c.Id == id, trackChanges)
        .SingleOrDefaultAsync();

    public async Task<Course?> GetCourseByIdAsync(Guid id, bool trackChanges) =>
        await FindByCondition(c => c.Id == id, trackChanges)
        .SingleOrDefaultAsync();

    public void UpdateCourse(Course course) => Update(course);
}
