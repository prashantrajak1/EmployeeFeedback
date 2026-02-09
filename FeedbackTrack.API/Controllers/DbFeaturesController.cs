using FeedbackTrack.API.Data;
using FeedbackTrack.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeedbackTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DbFeaturesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DbFeaturesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Using the View (vw_UserProfileView)
        [HttpGet("user-profiles")]
        public async Task<ActionResult<IEnumerable<vw_UserProfileView>>> GetUserProfiles()
        {
            // Querying the view just like a table
            return await _context.UserProfiles.ToListAsync();
        }

        // 2. Using the Stored Procedure (sp_GetUserFeedbackStats)
        [HttpGet("feedback-stats/{userId}")]
        public async Task<ActionResult<sp_UserFeedbackStats>> GetUserFeedbackStats(int userId)
        {
            // Calling the stored procedure using FromSqlRaw or SqlQuery
            var stats = await _context.UserFeedbackStats
                .FromSqlRaw("EXEC sp_GetUserFeedbackStats @UserId = {0}", userId)
                .ToListAsync();

            var result = stats.FirstOrDefault();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
