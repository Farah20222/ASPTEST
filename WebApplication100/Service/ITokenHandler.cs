using WebApplication100.Models;

namespace WebApplication100.Service
{
    public interface ITokenHandler
    {
        Task<string> CreateTokenAsync(UserProfile user);

    }
}
