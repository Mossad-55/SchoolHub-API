using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolHubAPI.Entities.Entities;

public class Attendance
{
    [Key]
    public Guid? Id { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? Status { get; set; }

    public Guid BatchId { get; set; }
    public Guid StudentId { get; set; }
    public Guid MarkedByTeacherId { get; set; }

    [ForeignKey("BatchId")]
    public Batch? Batch { get; set; }
    [ForeignKey("StudentId")]
    public Student? Student { get; set; }
    [ForeignKey("MarkedByTeacherId")]
    public Teacher? Teacher { get; set; }
}
