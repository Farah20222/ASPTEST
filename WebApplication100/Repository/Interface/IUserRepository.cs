using WebApplication100.Models;

namespace WebApplication100.Repository.Interface
{
    public interface IUserRepository
    {
        Task<UserProfile> AuthenticateAsync(string email, string password);
        Task<UserProfile> ChangePassAsync(int? userId, string newPassword);
        Task<UserProfile> GetByEmailAsync(string email);
        Task<UserProfile> AddChangePassTokenAsync(UserProfile user, string token, DateTime date);
        Task<UserProfile> ForgetPasswordAsync(UserProfile user, string password);
        Task<UserProfile> RegisterAsync(UserProfile user);
        Task<bool> VerifyCurrentPasswordAsync(int userId, string currentPassword);
        Task<UserProfile> GetByUserId(int? userId);
        Task<UserProfile> GetByTokenAsync(string token);
    }
}
