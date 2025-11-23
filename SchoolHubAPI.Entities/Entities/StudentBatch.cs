using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolHubAPI.Entities.Entities;

public class StudentBatch
{
    [Key]
    public Guid Id { get; set; }
    public DateTime? EnrollmentDate { get; set; }

    public Guid StudentId { get; set; }
    public Guid BatchId { get; set; }

    [ForeignKey("StudentId")]
    public Student? Student { get; set; }
    [ForeignKey("DepartmentId")]
    public Batch? Batch { get; set; }
}
