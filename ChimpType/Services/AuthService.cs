using ChimpType.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ChimpType.Services
{
    public class AuthService
    {
        private readonly ChimpTypeDbContext _context;
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public AuthService(ChimpTypeDbContext context) => _context = context;

        public async Task<User?> Register(string username, string email, string password, string name)
        {
            var exising = await _context.Users.FirstOrDefaultAsync(x => x.Username == username || x.Email == email);
            if (exising != null) return null;

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                Name = name,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                IsAdmin = false,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> Login(string usernameOrEmail, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == usernameOrEmail || x.Email == usernameOrEmail);
            if (user?.Active != true || !VerifyPassword(password, user.PasswordHash)) 
                return null;

            var tokenBytes = new byte[32];
            _rng.GetBytes(tokenBytes);
            var token = Convert.ToBase64String(tokenBytes);

            user.SessionTokenId = token;
            user.SessionExpires = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> ValidateSessionToken(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.SessionTokenId == token && x.SessionExpires > DateTime.UtcNow);
        }

        public async Task Logout(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user != null)
            {
                user.SessionTokenId = null;
                user.SessionExpires = null;
                await _context.SaveChangesAsync();
            }
        }

        private string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(128 / 8);
            var hashed = KeyDerivation.Pbkdf2(
                password: password, 
                salt: salt, 
                prf: KeyDerivationPrf.HMACSHA256, 
                iterationCount: 100000, 
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(hashed.Concat(salt).ToArray());
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashBytes = Convert.FromBase64String(hash);
            var salt = hashBytes.Skip(256 / 8).Take(128 / 8).ToArray();
            var hashed = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8);
            return hashBytes.Take(256 / 8).SequenceEqual(hashed);
        }

        internal async Task<User> ConfirmSessionToken(string username, string sessionToken, DateTime sessionExpied)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user != null)
            {
                if (user.SessionExpires > DateTime.UtcNow && user.SessionTokenId == sessionToken)
                {
                    return user;
                }
                else
                {
                    throw new UnauthorizedAccessException("Invalid or expired session token.");
                }
            }
            
            throw new UnauthorizedAccessException("User not found.");
        }
    }
}
