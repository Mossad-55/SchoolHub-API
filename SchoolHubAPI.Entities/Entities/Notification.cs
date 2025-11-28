using SchoolHubAPI.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolHubAPI.Entities.Entities;

public class Notification
{
    [Key]
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public RecipientRole RecipientRole { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
    public Guid? RecipientId { get; set; }
}
