using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;

namespace SchoolApi.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly int _refreshTokenLength = 64;
        private readonly int _refreshTokenExpiryDays = 7;

        public RefreshTokenService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            var token = GenerateSecureToken(_refreshTokenLength);
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = token,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
                CreatedDate = DateTime.UtcNow,
                IsRevoked = false
            };
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens.Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task InvalidateRefreshTokenAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            token.RevokedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken oldToken)
        {
            await InvalidateRefreshTokenAsync(oldToken);
            return await GenerateRefreshTokenAsync(oldToken.User);
        }

        public async Task RemoveExpiredTokensAsync()
        {
            var expiredTokens = await _context.RefreshTokens
                .Where(rt => rt.ExpiryDate < DateTime.UtcNow || rt.IsRevoked)
                .ToListAsync();
            if (expiredTokens.Any())
            {
                _context.RefreshTokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }
        }

        private static string GenerateSecureToken(int length)
        {
            var randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }
}