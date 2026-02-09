namespace FeedbackTrack.API.DTOs
{
    public class RecognitionCreateDto
    {
        public int ToUserId { get; set; }
        public string BadgeType { get; set; } = string.Empty;
        public int Points { get; set; }
        public string Comments { get; set; } = string.Empty;
    }
}
