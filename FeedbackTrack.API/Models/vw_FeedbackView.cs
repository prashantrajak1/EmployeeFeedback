using System.ComponentModel.DataAnnotations;

namespace FeedbackTrack.API.Models
{
    public class vw_FeedbackView
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public string? FromUserName { get; set; }
        public int ToUserId { get; set; }
        public string? ToUserName { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
        public DateTime Date { get; set; }
    }
}
