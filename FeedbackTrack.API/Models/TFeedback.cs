using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrack.API.Models
{
    public class TFeedback
    {
        [Key]
        public int Id { get; set; }
        
        public int? FromUserId { get; set; }
        [ForeignKey("FromUserId")]
        public TUser? FromUser { get; set; }

        public int ToUserId { get; set; }
        [ForeignKey("ToUserId")]
        public TUser? ToUser { get; set; }

        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsAnonymous { get; set; } = false;
    }
}
