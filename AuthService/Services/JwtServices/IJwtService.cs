using System.Threading.Tasks;
using AuthService.Domain;

namespace AuthService.Services.JwtServices
{
    public interface IJwtService
    {
        Task<string> GenerateAsync(User user);
    }
}
