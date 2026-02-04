using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrack.API.Models
{
    public class TUser
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;

        // Foreign Keys
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public TRole? Role { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public TDepartment? Department { get; set; }
    }
}
