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
    public class RecognitionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecognitionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SendRecognition(RecognitionCreateDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC usp_AddRecognition @p0, @p1, @p2, @p3, @p4, @p5",
                userId, dto.ToUserId, dto.BadgeType, dto.Points, dto.Comments, DateTime.UtcNow);

            // Trigger notification for the recipient
            var fromUser = await _context.TUsers.FindAsync(userId);
            var notificationMsg = $"You received a new Recognition from {fromUser?.Name}! ({dto.BadgeType})";
            await _context.Database.ExecuteSqlRawAsync("EXEC usp_Notification_Create @p0, @p1, @p2",
                notificationMsg, dto.ToUserId, DateTime.UtcNow);
            
            return Ok(new { message = "Recognition sent successfully" });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRecognitions(int userId)
        {
            var recognitions = await _context.RecognitionViews
                .FromSqlRaw("EXEC usp_GetRecognition @p0", userId)
                .ToListAsync();
            return Ok(recognitions);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var leaderboard = await _context.RecognitionViews
                .GroupBy(r => r.ToUserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalPoints = g.Sum(r => r.Points),
                    UserName = g.First().ToUserName
                })
                .OrderByDescending(x => x.TotalPoints)
                .Take(10)
                .ToListAsync();
            
            return Ok(leaderboard);
        }
    }
}
