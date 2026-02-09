namespace FeedbackTrack.API.DTOs
{
    public class FeedbackCreateDto
    {
        public int ToUserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; } = false;
    }
}
