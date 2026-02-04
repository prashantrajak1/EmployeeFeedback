using System.ComponentModel.DataAnnotations;

namespace FeedbackTrack.API.Models
{
    public class TRole
    {
        [Key]
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty; // Admin, Manager, Employee
    }
}
