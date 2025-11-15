using SchoolHubAPI.Entities.Entities;

namespace SchoolHubAPI.Contracts;

public interface IStudentRepository
{
    Task<List<Student>>? GetAllStudentsAsync(bool trackChanges);
    Task<Student?> GetStudentAsync(Guid id, bool trackChanges);
    void DeleteStudentAsync(Student student);
    void CreateStudentAsync(Student student);
    void UpdateStudentAsync(Student student);
}
