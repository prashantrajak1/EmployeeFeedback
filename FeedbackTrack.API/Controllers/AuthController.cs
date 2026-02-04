using FeedbackTrack.API.DTOs;
using FeedbackTrack.API.Models;
using FeedbackTrack.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FeedbackTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = await _userService.RegisterAsync(dto);
            if (user == null)
            {
                return BadRequest("User with this email already exists.");
            }
            return Ok(new { message = "Registration successful", userId = user.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto);
            if (token == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            return Ok(new { token });
        }

        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin()
        {
            // For "strictly" requested requirement: Admin should be logined automatically.
            // We search for the first Admin user or create one if none exists.
            var adminUser = await _userService.GetAllUsersAsync();
            var admin = adminUser.FirstOrDefault(u => u.Role?.RoleName == "Admin");

            if (admin == null)
            {
                // If no admin exists, we can't login, but for a demo, we might want to return a dummy or create one.
                // For now, let's assume seed data worked.
                return BadRequest("No Admin user found in database. Please register one first.");
            }

            // Generate token for this admin
            var token = await _userService.LoginAsync(new LoginDto { Email = admin.Email, Password = admin.Password });
            return Ok(new { token });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}
