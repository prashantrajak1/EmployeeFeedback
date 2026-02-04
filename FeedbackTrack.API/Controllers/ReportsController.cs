using FeedbackTrack.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}
