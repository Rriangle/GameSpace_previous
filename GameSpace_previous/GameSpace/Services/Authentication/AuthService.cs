using GameSpace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GameSpace.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            GameSpacedatabaseContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Check if account already exists
                if (await _context.Users.AnyAsync(u => u.UserAccount == request.UserAccount))
                {
                    return new AuthResult { Success = false, ErrorMessage = "Account already exists" };
                }

                // Check if email already exists
                if (await _context.UserIntroduces.AnyAsync(ui => ui.Email == request.Email))
                {
                    return new AuthResult { Success = false, ErrorMessage = "Email already exists" };
                }

                // Hash password
                var hashedPassword = HashPassword(request.UserPassword);

                // Create user
                var user = new User
                {
                    UserAccount = request.UserAccount,
                    UserPassword = hashedPassword,
                    UserEmailConfirmed = false,
                    UserPhoneNumberConfirmed = false,
                    UserTwoFactorEnabled = false,
                    UserAccessFailedCount = 0,
                    UserLockoutEnabled = false,
                    UserLockoutEnd = null
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create user introduce
                var userIntroduce = new UserIntroduce
                {
                    UserId = user.UserId,
                    UserNickName = request.UserNickName,
                    Gender = request.Gender,
                    IdNumber = request.IdNumber,
                    Cellphone = request.Cellphone,
                    Email = request.Email,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
                    CreateAccount = DateTime.UtcNow
                };

                _context.UserIntroduces.Add(userIntroduce);

                // Create user rights
                var userRight = new UserRight
                {
                    UserId = user.UserId,
                    UserStatus = true,
                    ShoppingPermission = true,
                    MessagePermission = true,
                    SalesAuthority = false
                };

                _context.UserRights.Add(userRight);

                // Create user wallet
                var userWallet = new UserWallet
                {
                    UserId = user.UserId,
                    UserPoint = 0
                };

                _context.UserWallets.Add(userWallet);

                await _context.SaveChangesAsync();

                // Generate tokens
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Store refresh token
                var userToken = new UserToken
                {
                    UserId = user.UserId,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(30),
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserTokens.Add(userToken);
                await _context.SaveChangesAsync();

                return new AuthResult
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return new AuthResult { Success = false, ErrorMessage = "Registration failed" };
            }
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRight)
                    .FirstOrDefaultAsync(u => u.UserAccount == request.Account);

                if (user == null)
                {
                    return new AuthResult { Success = false, ErrorMessage = "Invalid credentials" };
                }

                // Check if user is locked
                if (user.UserLockoutEnabled && user.UserLockoutEnd > DateTime.UtcNow)
                {
                    return new AuthResult { Success = false, ErrorMessage = "Account is locked" };
                }

                // Check if user is active
                if (user.UserRight?.UserStatus != true)
                {
                    return new AuthResult { Success = false, ErrorMessage = "Account is disabled" };
                }

                // Verify password
                if (!VerifyPassword(request.Password, user.UserPassword))
                {
                    // Increment failed count
                    user.UserAccessFailedCount++;
                    if (user.UserAccessFailedCount >= 5)
                    {
                        user.UserLockoutEnabled = true;
                        user.UserLockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    }
                    await _context.SaveChangesAsync();

                    return new AuthResult { Success = false, ErrorMessage = "Invalid credentials" };
                }

                // Reset failed count on successful login
                user.UserAccessFailedCount = 0;
                user.UserLockoutEnabled = false;
                user.UserLockoutEnd = null;
                await _context.SaveChangesAsync();

                // Generate tokens
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Store refresh token
                var userToken = new UserToken
                {
                    UserId = user.UserId,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(30),
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserTokens.Add(userToken);
                await _context.SaveChangesAsync();

                return new AuthResult
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return new AuthResult { Success = false, ErrorMessage = "Login failed" };
            }
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var userToken = await _context.UserTokens
                    .Include(ut => ut.User)
                    .FirstOrDefaultAsync(ut => ut.Token == refreshToken && ut.ExpiresAt > DateTime.UtcNow);

                if (userToken == null)
                {
                    return new AuthResult { Success = false, ErrorMessage = "Invalid refresh token" };
                }

                var user = userToken.User;
                if (user == null)
                {
                    return new AuthResult { Success = false, ErrorMessage = "User not found" };
                }

                // Generate new tokens
                var newToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                // Update refresh token
                userToken.Token = newRefreshToken;
                userToken.ExpiresAt = DateTime.UtcNow.AddDays(30);
                userToken.CreatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new AuthResult
                {
                    Success = true,
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return new AuthResult { Success = false, ErrorMessage = "Token refresh failed" };
            }
        }

        public async Task<bool> LogoutAsync(string token)
        {
            try
            {
                var userToken = await _context.UserTokens
                    .FirstOrDefaultAsync(ut => ut.Token == token);

                if (userToken != null)
                {
                    _context.UserTokens.Remove(userToken);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return false;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserRight)
                .Include(u => u.UserIntroduce)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByAccountAsync(string account)
        {
            return await _context.Users
                .Include(u => u.UserRight)
                .Include(u => u.UserIntroduce)
                .FirstOrDefaultAsync(u => u.UserAccount == account);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                if (!VerifyPassword(currentPassword, user.UserPassword))
                {
                    return false;
                }

                user.UserPassword = HashPassword(newPassword);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                var userIntroduce = await _context.UserIntroduces
                    .Include(ui => ui.User)
                    .FirstOrDefaultAsync(ui => ui.Email == email);

                if (userIntroduce?.User == null) return false;

                // Generate reset token
                var resetToken = GenerateRefreshToken();
                var userToken = new UserToken
                {
                    UserId = userIntroduce.User.UserId,
                    Token = resetToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserTokens.Add(userToken);
                await _context.SaveChangesAsync();

                // TODO: Send email with reset token
                _logger.LogInformation($"Password reset token for {email}: {resetToken}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return false;
            }
        }

        public async Task<bool> ConfirmEmailAsync(int userId, string token)
        {
            try
            {
                var userToken = await _context.UserTokens
                    .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.Token == token && ut.ExpiresAt > DateTime.UtcNow);

                if (userToken == null) return false;

                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.UserEmailConfirmed = true;
                _context.UserTokens.Remove(userToken);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email");
                return false;
            }
        }

        public async Task<bool> LockUserAsync(int userId, DateTime? lockoutEnd)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.UserLockoutEnabled = true;
                user.UserLockoutEnd = lockoutEnd;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking user");
                return false;
            }
        }

        public async Task<bool> UnlockUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.UserLockoutEnabled = false;
                user.UserLockoutEnd = null;
                user.UserAccessFailedCount = 0;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking user");
                return false;
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserAccount),
                    new Claim("UserId", user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}