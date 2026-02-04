using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrack.API.Models
{
    public class TReview
    {
        [Key]
        public int Id { get; set; }
        
        public int FeedbackId { get; set; }
        [ForeignKey("FeedbackId")]
        public TFeedback? Feedback { get; set; }
        
        public int ReviewerId { get; set; } // Manager's Id
        
        public string Comments { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
