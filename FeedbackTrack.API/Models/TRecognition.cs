using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrack.API.Models
{
    public class TRecognition
    {
        [Key]
        public int Id { get; set; }

        public int FromUserId { get; set; }
        [ForeignKey("FromUserId")]
        public TUser? FromUser { get; set; }

        public int ToUserId { get; set; }
        [ForeignKey("ToUserId")]
        public TUser? ToUser { get; set; }

        public string BadgeType { get; set; } = string.Empty; // e.g., "Star Performer"
        public int Points { get; set; } = 0;
        public string Comments { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
