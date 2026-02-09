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

            var recognition = new TRecognition
            {
                FromUserId = userId,
                ToUserId = dto.ToUserId,
                BadgeType = dto.BadgeType,
                Points = dto.Points,
                Comments = dto.Comments,
                Date = DateTime.UtcNow
            };

            _context.TRecognitions.Add(recognition);
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Recognition sent successfully" });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRecognitions(int userId)
        {
            var recognitions = await _context.TRecognitions
                .Include(r => r.FromUser)
                .Where(r => r.ToUserId == userId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
            return Ok(recognitions);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var leaderboard = await _context.TRecognitions
                .GroupBy(r => r.ToUserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalPoints = g.Sum(r => r.Points),
                    User = g.First().ToUser
                })
                .OrderByDescending(x => x.TotalPoints)
                .Take(10)
                .ToListAsync();
            
            return Ok(leaderboard);
        }
    }
}
