using FeedbackTrack.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FeedbackTrack.API.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var totalFeedback = await _context.TFeedbacks.CountAsync();
                var totalRecognitions = await _context.TRecognitions.CountAsync();
                var activeUsers = await _context.TUsers.CountAsync(u => u.IsActive);

                return Ok(new
                {
                    TotalFeedback = totalFeedback,
                    TotalRecognitions = totalRecognitions,
                    ActiveUsers = activeUsers
                });
            }
            catch (Exception ex)
            {
                // Fallback to 0 if table missing / error (SQLite or EF issues)
                return Ok(new
                {
                    TotalFeedback = 0,
                    TotalRecognitions = 0,
                    ActiveUsers = 0
                });
            }
        }

        [HttpGet("trends")]
        public async Task<IActionResult> GetTrends()
        {
            try
            {
                var feedbackTrends = await _context.TFeedbacks
                    .GroupBy(f => f.Date.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .Take(30)
                    .ToListAsync();

                var recognitionTrends = await _context.TRecognitions
                    .GroupBy(r => r.Date.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .Take(30)
                    .ToListAsync();

                return Ok(new { feedbacks = feedbackTrends, recognitions = recognitionTrends });
            }
            catch
            {
                return Ok(new { feedbacks = new List<object>(), recognitions = new List<object>() });
            }
        }

        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportStatsCsv()
        {
            try
            {
                var feedbacks = await _context.TFeedbacks.OrderByDescending(f => f.Date).Take(100).ToListAsync();
                var recognitions = await _context.TRecognitions.OrderByDescending(r => r.Date).Take(100).ToListAsync();

                var csv = new StringBuilder();
                csv.AppendLine("Type,Date,From,To,Description/Badge,Points/Anonymous");

                foreach (var f in feedbacks)
                {
                    csv.AppendLine($"Feedback,{f.Date},{f.FromUserId},{f.ToUserId},\"{f.Description.Replace("\"", "\"\"")}\",{f.IsAnonymous}");
                }

                foreach (var r in recognitions)
                {
                    csv.AppendLine($"Kudos,{r.Date},{r.FromUserId},{r.ToUserId},{r.BadgeType},{r.Points}");
                }

                return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"FeedbackTrack_Report_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch
            {
                return BadRequest("Failed to generate report.");
            }
        }
        [HttpGet("member-report")]
        public async Task<IActionResult> GetMemberReport()
        {
            try
            {
                var users = await _context.TUsers.Include(u => u.Department).ToListAsync();
                var feedbacks = await _context.TFeedbacks.ToListAsync();
                var recognitions = await _context.TRecognitions.ToListAsync();

                var report = users.Select(u => new
                {
                    UserId = u.Id,
                    UserName = u.Name,
                    Department = u.Department?.DepartmentName ?? "Unknown",
                    IsActive = u.IsActive,
                    FeedbackSent = feedbacks.Count(f => f.FromUserId == u.Id),
                    FeedbackReceived = feedbacks.Count(f => f.ToUserId == u.Id),
                    KudosSent = recognitions.Count(r => r.FromUserId == u.Id),
                    KudosReceived = recognitions.Count(r => r.ToUserId == u.Id),
                    TotalPoints = recognitions.Where(r => r.ToUserId == u.Id).Sum(r => r.Points)
                }).OrderByDescending(x => x.TotalPoints).ToList();

                return Ok(report);
            }
            catch
            {
                return BadRequest("Failed to load member report.");
            }
        }

        [HttpGet("export-member-report")]
        public async Task<IActionResult> ExportMemberReportCsv()
        {
            try
            {
                var users = await _context.TUsers.Include(u => u.Department).ToListAsync();
                var feedbacks = await _context.TFeedbacks.ToListAsync();
                var recognitions = await _context.TRecognitions.ToListAsync();

                var report = users.Select(u => new
                {
                    UserId = u.Id,
                    UserName = u.Name,
                    Department = u.Department?.DepartmentName ?? "Unknown",
                    IsActive = u.IsActive ? "Active" : "Inactive",
                    FeedbackSent = feedbacks.Count(f => f.FromUserId == u.Id),
                    FeedbackReceived = feedbacks.Count(f => f.ToUserId == u.Id),
                    KudosSent = recognitions.Count(r => r.FromUserId == u.Id),
                    KudosReceived = recognitions.Count(r => r.ToUserId == u.Id),
                    TotalPoints = recognitions.Where(r => r.ToUserId == u.Id).Sum(r => r.Points),
                    // Add content aggregations
                    RecentFeedbacks = string.Join(" | ", feedbacks.Where(f => f.ToUserId == u.Id).Take(5).Select(f => f.Description.Replace("\"", "'").Replace("\n", " "))),
                    RecentKudos = string.Join(" | ", recognitions.Where(r => r.ToUserId == u.Id).Take(5).Select(r => $"[{r.BadgeType}] {r.Comments.Replace("\"", "'").Replace("\n", " ")}"))
                }).OrderByDescending(x => x.TotalPoints).ToList();

                var csv = new StringBuilder();
                csv.AppendLine("Name,Department,Status,Feedback Sent,Feedback Received,Kudos Sent,Kudos Received,Total Points,Recent FeedbacksContent,Recent KudosContent");

                foreach (var r in report)
                {
                    csv.AppendLine($"\"{r.UserName}\",\"{r.Department}\",{r.IsActive},{r.FeedbackSent},{r.FeedbackReceived},{r.KudosSent},{r.KudosReceived},{r.TotalPoints},\"{r.RecentFeedbacks}\",\"{r.RecentKudos}\"");
                }

                return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"TeamPulse_MemberReport_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch
            {
                return BadRequest("Failed to generate report.");
            }
        }
    }
}
