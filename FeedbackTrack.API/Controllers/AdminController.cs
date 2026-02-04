using FeedbackTrack.API.Data;
using FeedbackTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeedbackTrack.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("toggle-user/{id}")]
        public async Task<IActionResult> ToggleUser(int id)
        {
            var user = await _context.TUsers.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
            return Ok(new { message = $"User {user.Name} is now {(user.IsActive ? "Active" : "Inactive")}" });
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            // Placeholder: Returning hardcoded categories as they aren't in DB yet.
            // In a real app, these would be a table.
            var categories = new[] { "Collaboration", "Excellence", "Innovation", "Growth", "Ownership" };
            return Ok(categories);
        }
    }
}
