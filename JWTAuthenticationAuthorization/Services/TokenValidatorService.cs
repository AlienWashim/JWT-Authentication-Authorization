using JWTAuthenticationAuthorization;
using Microsoft.EntityFrameworkCore;

namespace WeatherAPI.Services
{
    public class TokenValidatorService
    {
        private readonly MyDBContext _context;

        public TokenValidatorService(MyDBContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateTokenAsync(string userEmail, string userPassword, string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return false;

            // Here you can compare the token from the database and the one provided in the request
            return user.Token == token;
        }
    }

}
