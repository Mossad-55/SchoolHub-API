using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolHubAPI.Entities.Entities;

public class Course
{
    [Key]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int Credits { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public Guid DepartmentId { get; set; }
    [ForeignKey("DepartmentId")]
    public Department? Department { get; set; }
}
