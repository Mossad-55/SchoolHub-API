using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolHubAPI.Entities.Entities;

public class Batch
{
    [Key]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Semester { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public Guid CourseId { get; set; }
    public Guid TeacherId { get; set; }

    [ForeignKey("CourseId")]
    public Course? Course { get; set; }
    [ForeignKey("TeacherId")]
    public Teacher? Teacher { get; set; }
    // Remember to put navigation property for students enrollment.
}
