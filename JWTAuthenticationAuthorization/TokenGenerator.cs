using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Core;
using JWTAuthenticationAuthorization.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuthenticationAuthorization
{
    public class TokenGenerator
    {
        private readonly UserService _userService;

        public TokenGenerator(UserService userService)
        {
            _userService = userService;  // This will be injected automatically by the DI container
        }

        public async Task<string> GenerateTokenAsync(string email, string password)
        {
            bool isNewUser = await _userService.IsNewUserAsync(email);

            string newToken = "";

            if (isNewUser)
            {
                newToken = GetNewToken(email, password);
                await _userService.CreateNewUserAsync(email, password, newToken);
                return newToken;
            }

            if (!isNewUser && await _userService.IsValidUserAsync(email, password))
            {
                bool isTokenExpired = await _userService.IsTokenExpiredAsync(email);

                if (!isTokenExpired)
                {
                    return await _userService.ReturnExistingTokenAsync(email);
                }
                else
                {
                    newToken = GetNewToken(email, password);
                    await _userService.UpdateUserAsync(email, password, newToken);
                    return newToken;
                }
            }
            return "Wrong email or password";
        }

        private string GetNewToken(string email, string password)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("IgyIXTuplbYrjSSvblDpmCXhBoMWVwLk");

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Email, email)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(20),
                Issuer = "https://localhost:7048/",
                Audience = "https://localhost:7100/",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


}
