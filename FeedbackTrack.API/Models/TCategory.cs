using System.ComponentModel.DataAnnotations;

namespace FeedbackTrack.API.Models
{
    public class TCategory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
