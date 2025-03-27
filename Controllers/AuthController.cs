using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EtaxService.Configuration;
using Microsoft.Extensions.Options;
using EtaxService.DTOs.Request;
using EtaxService.Database;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using EtaxService.Models;

namespace EtaxService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly EtaxDatabaseContext _dbContext;

        public AuthController(
            IOptions<JwtSettings> jwtSettings,
            EtaxDatabaseContext dbContext
            )
        {
            _jwtSettings = jwtSettings.Value;
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || request.Username == null || request.Password == null)
            {
                return BadRequest("Invalid request body.");
            }

            // Use a safer query approach with explicit column selection
            var user = await _dbContext.Users
                .Where(u => u.Username == request.Username)
                .Select(u => new User
                {
                    ID = u.ID,
                    Username = u.Username,
                    PasswordHash = u.PasswordHash,
                    PasswordSalt = u.PasswordSalt
                })
            .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // Check for NULL password hash or salt
            if (user.PasswordHash == null || user.PasswordSalt == null)
            {
                // Log this issue
                Console.WriteLine("User has NULL password hash or salt");
                return Unauthorized("Account setup incomplete. Please reset your password.");
            }


            // Convert Base64 strings back to byte arrays
            byte[] passwordHashBytes = Convert.FromBase64String(user.PasswordHash);
            byte[] passwordSaltBytes = Convert.FromBase64String(user.PasswordSalt);

            if (!VerifyPasswordHash(request.Password, passwordHashBytes, passwordSaltBytes))
            {
                return Unauthorized("Invalid credentials");
            }


            if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
            {
                return StatusCode(500, "JWT SecretKey is missing in configuration.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(_jwtSettings.Expire)),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }

        // Helper method to verify password hash
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (storedHash == null) throw new ArgumentNullException(nameof(storedHash));
            if (storedSalt == null) throw new ArgumentNullException(nameof(storedSalt));

            try
            {
                if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));
                if (storedSalt.Length != 128
                ) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", nameof(storedSalt));

                using (var hmac = new HMACSHA512(storedSalt))
                {
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != storedHash[i]) return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the specific error
                Console.WriteLine($"Password verification error: {ex.Message}");
                throw; // Re-throw to be handled by the calling method
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request body.");
            }

            // Check if username already exists
            if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("Username already exists.");
            }

            // Create password hash
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Create new user
            var user = new User
            {
                Username = request.Username,
                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = Convert.ToBase64String(passwordSalt),
                Role = request.Role ?? "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add user to database
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully" });
        }

        // Helper method to create password hash
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        [HttpGet("test-login")]
        public IActionResult TestLogin()
        {
            if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
            {
                return StatusCode(500, "JWT SecretKey is missing in configuration.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "test_user"),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(_jwtSettings.Expire)),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }
}
