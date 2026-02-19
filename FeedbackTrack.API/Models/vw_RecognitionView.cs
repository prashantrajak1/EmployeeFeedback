using System.ComponentModel.DataAnnotations;

namespace FeedbackTrack.API.Models
{
    public class vw_RecognitionView
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public string? FromUserName { get; set; }
        public int ToUserId { get; set; }
        public string? ToUserName { get; set; }
        public string BadgeType { get; set; } = string.Empty;
        public int Points { get; set; }
        public string Comments { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
