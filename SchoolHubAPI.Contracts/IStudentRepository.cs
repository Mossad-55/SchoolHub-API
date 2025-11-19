using SchoolHubAPI.Entities.Entities;
using SchoolHubAPI.Shared.RequestFeatures;

namespace SchoolHubAPI.Contracts;

public interface IStudentRepository
{
    Task<PagedList<Student>>? GetAllStudentsAsync(RequestParameters requestParameters, bool trackChanges);
    Task<Student?> GetStudentAsync(Guid id, bool trackChanges);
    void DeleteStudentAsync(Student student);
    void CreateStudentAsync(Student student);
    void UpdateStudentAsync(Student student);
}
