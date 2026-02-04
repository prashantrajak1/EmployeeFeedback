using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrack.API.Models
{
    public class TNotification
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; } // Who it is for
        public bool IsRead { get; set; } = false;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
