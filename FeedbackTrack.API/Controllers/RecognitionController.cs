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
    public class RecognitionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecognitionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SendRecognition(TRecognition recognition)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            recognition.FromUserId = userId;
            recognition.Date = DateTime.UtcNow;

            _context.TRecognitions.Add(recognition);
            await _context.SaveChangesAsync();
            return Ok(recognition);
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
