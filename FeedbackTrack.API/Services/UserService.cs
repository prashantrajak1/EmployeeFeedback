using FeedbackTrack.API.Data;
using FeedbackTrack.API.DTOs;
using FeedbackTrack.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FeedbackTrack.API.Services
{
    public class UserService : IUserService
    {
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
            
            // Find or Create Department (using Stored Procedure Logic Concept primarily, but standard EF here for simplicity unless strictly forced to SP for *everything*)
            // The prompt says "add department table stored procedure should be used". I'll try to fetch Department via SP.
            // Note: Switched to LINQ as SQLite does not support Stored Procedures.
            // Original Requirement: "department table stored procedure should be used" -> Not possible with SQLite.
            // We use raw SQL to stay close to the "native query" spirit if needed, or just LINQ.
            // Using LINQ for reliability here.
            var department = await _context.TDepartments
                .FirstOrDefaultAsync(d => d.DepartmentName == dto.Department);

            if (department == null)
            {
               // If SP doesn't find it, maybe create it? Or require valid departments. 
               // For now, let's create it if missing, or use default.
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

        public async Task<string> LoginAsync(LoginDto dto)
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
            return tokenHandler.WriteToken(token);
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
    }
}
