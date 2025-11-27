using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolHubAPI.Entities.Entities;

public class Submission
{
    [Key]
    public Guid Id { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string? FileUrl { get; set; }
    public decimal? Grade { get; set; }
    public string? Remarks { get; set; }

    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public Guid? GradedByTeacherId { get; set; }

    [ForeignKey("AssignmentId")]
    public Assignment? Assignment { get; set; }
    [ForeignKey("StudentId")]
    public Student? Student { get; set; }
    [ForeignKey("GradedByTeacherId")]
    public Teacher? Teacher { get; set; }
}
