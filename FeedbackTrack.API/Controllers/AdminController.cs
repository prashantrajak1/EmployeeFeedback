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
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.TCategories.Select(c => c.Name).ToListAsync();
            return Ok(categories);
        }

        public class CategoryDto { public string Name { get; set; } = string.Empty; }

        [HttpPost("categories")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest();
            if (await _context.TCategories.AnyAsync(c => c.Name == dto.Name)) return Ok(); // Idempotent
            
            _context.TCategories.Add(new TCategory { Name = dto.Name });
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("categories/{name}")]
        public async Task<IActionResult> DeleteCategory(string name)
        {
            var category = await _context.TCategories.FirstOrDefaultAsync(c => c.Name == name);
            if (category != null)
            {
                _context.TCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpGet("all-feedbacks")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacks = await _context.FeedbackViews
                .OrderByDescending(f => f.Date)
                .ToListAsync();
            return Ok(feedbacks);
        }

        [HttpGet("all-recognitions")]
        public async Task<IActionResult> GetAllRecognitions()
        {
            var recognitions = await _context.RecognitionViews
                .OrderByDescending(r => r.Date)
                .ToListAsync();
            return Ok(recognitions);
        }
    }
}
