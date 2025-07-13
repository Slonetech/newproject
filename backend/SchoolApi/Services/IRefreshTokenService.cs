using SchoolApi.Models;
using System.Threading.Tasks;

namespace SchoolApi.Services
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshTokenAsync(ApplicationUser user);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task InvalidateRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken oldToken);
        Task RemoveExpiredTokensAsync();
    }
}