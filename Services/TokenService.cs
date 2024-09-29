using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GameStore.Api.Services
{ 
    public static class TokenService
    {
        // Updated secret key to be 256 bits
        private static string _secretKey = "thisisaverylongsecretkeythatwillbeusedforjwtanditneedstobelongenough"; 

        // Token expiry duration
        private static int _expiryDurationInMinutes = 60;

        public static string GenerateToken(string username)
        {
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256); // Use HmacSha256

            var claims = new[] {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: "issuer123",
                audience: "audience123",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryDurationInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}