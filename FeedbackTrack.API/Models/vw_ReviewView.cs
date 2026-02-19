using System.ComponentModel.DataAnnotations;

namespace FeedbackTrack.API.Models
{
    public class vw_ReviewView
    {
        public int Id { get; set; }
        public int FeedbackId { get; set; }
        public string? FeedbackDescription { get; set; }
        public int ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
        public string Comments { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
