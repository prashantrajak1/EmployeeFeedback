using FeedbackTrack.API.Data;
using FeedbackTrack.API.DTOs;
using FeedbackTrack.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using System.Collections.Concurrent;

namespace FeedbackTrack.API.Services
{
    public class UserService : IUserService
    {
        private static readonly ConcurrentDictionary<int, DateTime> _activeSessions = new();
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TUser?> RegisterAsync(RegisterDto dto)
        {
            if (await _context.TUsers.AnyAsync(u => u.Email == dto.Email))
            {
                return null; // User already exists
            }

            // Find Role
            var role = await _context.TRoles.FirstOrDefaultAsync(r => r.RoleName == dto.Role);
            if (role == null)
            {
                // Fallback to Employee if role invalid
                role = await _context.TRoles.FirstOrDefaultAsync(r => r.RoleName == "Employee");
            }
            
            var department = await _context.TDepartments
                .FirstOrDefaultAsync(d => d.DepartmentName == dto.Department);

            if (department == null)
            {
               department = new TDepartment { DepartmentName = dto.Department };
               _context.TDepartments.Add(department);
               await _context.SaveChangesAsync();
            }

            var user = new TUser
            {
                Name = dto.Name,
                Email = dto.Email,
                RoleId = role.Id,
                DepartmentId = department.Id,
                Password = dto.Password // In production, hash this!
            };

            _context.TUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.TUsers
                .Include(u => u.Role)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || user.Password != dto.Password) 
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "SuperSecretKeyForDevelopmentOnly12345!"); 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Employee"),
                    new Claim("Department", user.Department?.DepartmentName ?? "Unknown")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            RecordActivity(user.Id); // Track login activity

            return new LoginResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                User = new UserProfileDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role?.RoleName ?? "Employee"
                }
            };
        }

        public async Task<TUser?> GetUserByIdAsync(int id)
        {
            return await _context.TUsers
                .Include(u => u.Role)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<TUser>> GetAllUsersAsync()
        {
            return await _context.TUsers
                .Include(u => u.Role)
                .Include(u => u.Department)
                .ToListAsync();
        }

        public void RecordActivity(int userId)
        {
            _activeSessions[userId] = DateTime.UtcNow;
        }

        public List<int> GetActiveUserIds()
        {
            var cutoff = DateTime.UtcNow.AddHours(-1);
            return _activeSessions
                .Where(kv => kv.Value >= cutoff)
                .Select(kv => kv.Key)
                .ToList();
        }
    }
}
