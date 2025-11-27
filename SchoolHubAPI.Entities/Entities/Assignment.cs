using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolHubAPI.Entities.Entities;

public class Assignment
{
    [Key]
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
    
    public Guid CreatedByTeacherId { get; set; }
    public Guid BatchId { get; set; }

    [ForeignKey("CreatedByTeacherId")]
    public Teacher? Teacher { get; set; }
    [ForeignKey("BatchId")]
    public Batch? Batch { get; set; }

    // Navigation
    // public virtual ICollection<Submission>? Submissions { get; set; }
}
