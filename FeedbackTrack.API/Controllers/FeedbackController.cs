using FeedbackTrack.API.Data;
using FeedbackTrack.API.DTOs;
using FeedbackTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FeedbackTrack.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(FeedbackCreateDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);

            var feedback = new TFeedback
            {
                FromUserId = userId,
                ToUserId = dto.ToUserId,
                Description = dto.Description,
                IsAnonymous = dto.IsAnonymous,
                Date = DateTime.UtcNow
            };

            _context.TFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Trigger notification for the recipient
            var fromUser = await _context.TUsers.FindAsync(userId);
            var notificationMsg = feedback.IsAnonymous 
                ? "You received new anonymous feedback." 
                : $"You received new feedback from {fromUser?.Name}.";

            await _context.Database.ExecuteSqlRawAsync("EXEC usp_Notification_Create @p0, @p1, @p2", 
                notificationMsg, feedback.ToUserId, DateTime.UtcNow);
            
            return Ok(new { message = "Feedback submitted successfully" });
        }

        [HttpGet("my-feedback")]
        public async Task<IActionResult> GetMyReceivedFeedback()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var feedbacks = await _context.FeedbackViews
                .Where(f => f.ToUserId == userId)
                .OrderByDescending(f => f.Date)
                .ToListAsync();

            // Anonymize if needed
            foreach (var f in feedbacks)
            {
                if (f.IsAnonymous)
                {
                    f.FromUserName = "Anonymous";
                    f.FromUserId = 0;
                }
            }

            return Ok(feedbacks);
        }

        // For managers to see feedback of their team
        [HttpGet("team/{userId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetTeamMemberFeedback(int userId)
        {
            var feedbacks = await _context.FeedbackViews
                .Where(f => f.ToUserId == userId)
                .OrderByDescending(f => f.Date)
                .ToListAsync();

             foreach (var f in feedbacks)
            {
                if (f.IsAnonymous)
                {
                    f.FromUserName = "Anonymous";
                    f.FromUserId = 0;
                }
            }

            return Ok(feedbacks);
        }

        [HttpGet("reviews/user/{userId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetReviewsByUser(int userId)
        {
            var reviews = await _context.TReviews
                .FromSqlRaw("EXEC usp_Review_GetByUserId @p0", userId)
                .ToListAsync();
            return Ok(reviews);
        }

        [HttpGet("my-reviews")]
        public async Task<IActionResult> GetMyReviews()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            // Get all reviews for feedbacks received by this user
            var reviews = await _context.TReviews
                .Include(r => r.Feedback)
                .Where(r => r.Feedback!.ToUserId == userId)
                .ToListAsync();

            // We only need to return the review status to the employee, but returning full object is fine
            return Ok(reviews);
        }

        [HttpPost("review")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> SubmitReview(ReviewCreateDto dto)
        {
            var reviewerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC usp_Review_Insert @p0, @p1, @p2, @p3",
                dto.FeedbackId, reviewerId, dto.Comments, DateTime.UtcNow);

            return Ok(new { message = "Review added successfully" });
        }
    }
}
