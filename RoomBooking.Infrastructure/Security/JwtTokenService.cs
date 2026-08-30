using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RoomBooking.Application.Users;
using RoomBooking.Domain.Users;

namespace RoomBooking.Infrastructure.Security
{
    public sealed class JwtTokenService : ITokenService
    {
        // HS256 підписується ключем не коротшим за 256 біт.
        private const int MinKeyBytes = 32;

        private const int DefaultExpiresMinutes = 60;

        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, int ExpiresInSeconds) CreateAccessToken(User user)
        {
            int minutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out int parsed)
                ? parsed
                : DefaultExpiresMinutes;

            DateTime now = DateTime.UtcNow;

            Claim[] claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            SigningCredentials credentials = new(
                new SymmetricSecurityKey(GetKeyBytes(_configuration)),
                SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(minutes),
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), minutes * 60);
        }

        // Ключ читає і цей сервіс, і перевірка токена в Program.cs.
        // public static, бо цей самий ключ потрібен у Program.cs для перевірки токенів.
        public static byte[] GetKeyBytes(IConfiguration configuration)
        {
            string key = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is missing.");

            byte[] bytes = Convert.FromBase64String(key);

            if (bytes.Length < MinKeyBytes)
            {
                throw new InvalidOperationException($"Jwt:Key must be at least {MinKeyBytes} bytes for HS256.");
            }

            return bytes;
        }
    }
}