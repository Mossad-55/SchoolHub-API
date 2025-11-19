using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolHubAPI.Entities.Entities;

public class Department
{
    [Key]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public Guid? HeadOfDepartmentId { get; set; }
    [ForeignKey("HeadOfDepartmentId")]
    public Teacher? HeadOfDepartment { get; set; }
}
