namespace FeedbackTrack.API.DTOs
{
    public class ReviewCreateDto
    {
        public int FeedbackId { get; set; }
        public string Comments { get; set; } = string.Empty;
    }
}
