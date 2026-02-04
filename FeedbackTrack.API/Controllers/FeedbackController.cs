using FeedbackTrack.API.Data;
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
        public async Task<IActionResult> SubmitFeedback(TFeedback feedback)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            feedback.FromUserId = userId; // Enforce sender
            feedback.Date = DateTime.UtcNow;

            _context.TFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return Ok(feedback);
        }

        [HttpGet("my-feedback")]
        public async Task<IActionResult> GetMyReceivedFeedback()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var feedbacks = await _context.TFeedbacks
                .Include(f => f.FromUser)
                .Where(f => f.ToUserId == userId)
                .OrderByDescending(f => f.Date)
                .ToListAsync();

            // Anonymize if needed
            foreach (var f in feedbacks)
            {
                if (f.IsAnonymous)
                {
                    f.FromUser = null;
                    f.FromUserId = null;
                }
            }

            return Ok(feedbacks);
        }

        // For managers to see feedback of their team
        [HttpGet("team/{userId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetTeamMemberFeedback(int userId)
        {
            // In a real app, verify that 'userId' actually reports to the current manager
            var feedbacks = await _context.TFeedbacks
                .Include(f => f.FromUser)
                .Where(f => f.ToUserId == userId)
                .OrderByDescending(f => f.Date)
                .ToListAsync();

             foreach (var f in feedbacks)
            {
                if (f.IsAnonymous)
                {
                    f.FromUser = null;
                    f.FromUserId = null;
                }
            }

            return Ok(feedbacks);
        }
    }
}
