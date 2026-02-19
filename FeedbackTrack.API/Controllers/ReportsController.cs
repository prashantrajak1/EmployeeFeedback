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
            var stats = await _context.ReportStats
                .FromSqlRaw("EXEC usp_GenerateReport")
                .ToListAsync();

            var result = stats.FirstOrDefault();

            return Ok(new
            {
                TotalFeedback = result?.TotalFeedback ?? 0,
                TotalRecognitions = result?.TotalRecognitions ?? 0,
                ActiveUsers = result?.ActiveUsers ?? 0
            });
        }

        [HttpGet("trends")]
        public async Task<IActionResult> GetTrends()
        {
            // Use .Date for grouping to avoid time issues. 
            // In SQL Server, this should translate to CAST(Date AS DATE)
            var feedbackTrends = await _context.FeedbackViews
                .GroupBy(f => f.Date.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .Take(30)
                .ToListAsync();

            var recognitionTrends = await _context.RecognitionViews
                .GroupBy(r => r.Date.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .Take(30)
                .ToListAsync();

            return Ok(new { feedbacks = feedbackTrends, recognitions = recognitionTrends });
        }

        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportStatsCsv()
        {
            var feedbacks = await _context.FeedbackViews.OrderByDescending(f => f.Date).Take(100).ToListAsync();
            var recognitions = await _context.RecognitionViews.OrderByDescending(r => r.Date).Take(100).ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Type,Date,From,To,Description/Badge,Points/Anonymous");

            foreach (var f in feedbacks)
            {
                csv.AppendLine($"Feedback,{f.Date},{f.FromUserName},{f.ToUserName},\"{f.Description.Replace("\"", "\"\"")}\",{f.IsAnonymous}");
            }

            foreach (var r in recognitions)
            {
                csv.AppendLine($"Kudos,{r.Date},{r.FromUserName},{r.ToUserName},{r.BadgeType},{r.Points}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"FeedbackTrack_Report_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}
