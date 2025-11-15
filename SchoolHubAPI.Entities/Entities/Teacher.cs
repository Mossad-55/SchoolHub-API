using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolHubAPI.Entities.Entities;

public class Teacher
{
    [Key]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
