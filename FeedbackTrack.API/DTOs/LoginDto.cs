namespace FeedbackTrack.API.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // In a real app, use password hashing. Here assuming simple auth for MVP.
    }
}
