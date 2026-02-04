using System.ComponentModel.DataAnnotations;

namespace FeedbackTrack.API.Models
{
    public class TDepartment
    {
        [Key]
        public int Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}
