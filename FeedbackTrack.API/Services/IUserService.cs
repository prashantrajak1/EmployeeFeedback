using FeedbackTrack.API.Models;
using FeedbackTrack.API.DTOs;

namespace FeedbackTrack.API.Services
{
    public interface IUserService
    {
        Task<TUser?> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
        Task<TUser?> GetUserByIdAsync(int id);
        Task<List<TUser>> GetAllUsersAsync();
        void RecordActivity(int userId);
        Task<List<int>> GetActiveUserIdsAsync(); // Ensure consistency if needed, wait let me check IUserService
        Task<List<string>> GetDepartmentsAsync();
    }
}
