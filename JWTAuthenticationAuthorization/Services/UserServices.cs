using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using JWTAuthenticationAuthorization.Models;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthenticationAuthorization.Services
{
    public class UserService
    {
        private readonly MyDBContext _context;

        public UserService(MyDBContext context)
        {
            _context = context;
        }

        public async Task CreateNewUserAsync(string email, string password, string token)
        {

            var passwordHash = HashPassword(password);

            var newUser = new UserEntity
            {
                Email = email,
                PasswordHash = passwordHash,
                Token = token,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(string email, string password, string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                user.Token = token;
                user.UpdatedAt = DateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsNewUserAsync(string userEmail)
        {
            // Fetch the user from the database by email
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);

            // If the user is not found, return false
            if (user == null)
                return true;
            return false;
        }

        public async Task<bool> IsValidUserAsync(string userEmail, string userPassword)
        {
            // Fetch the user from the database by email
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);

            // If the user is not found, return false
            if (user != null && user.PasswordHash == HashPassword(userPassword))
                return true;
            return false;
        }

        public async Task<string> ReturnExistingTokenAsync(string userEmail)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);
            return user?.Token;
        }

        public async Task<bool> IsTokenExpiredAsync(string userEmail)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail);

            if (user == null || string.IsNullOrEmpty(user.Token))
                return true;

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = tokenHandler.ReadJwtToken(user.Token);
                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch (Exception)
            {
                return true;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }


}
