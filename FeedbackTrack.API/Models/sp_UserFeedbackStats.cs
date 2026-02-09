namespace FeedbackTrack.API.Models
{
    public class sp_UserFeedbackStats
    {
        public int UserId { get; set; }
        public int FeedbackReceived { get; set; }
        public int FeedbackGiven { get; set; }
    }
}
