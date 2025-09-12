using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using GameSpace.Services.Authentication;
using GameSpace.Models;

namespace GameSpace.Tests.Services
{
    public class AuthServiceTests : IDisposable
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<GameSpacedatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameSpacedatabaseContext(options);
            _loggerMock = new Mock<ILogger<AuthService>>();
            _authService = new AuthService(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var user = new User
            {
                UserAccount = "testuser",
                UserName = "Test User",
                UserPassword = "TestPassword123!"
            };

            var userIntroduce = new UserIntroduce
            {
                UserNickName = "TestNick",
                Gender = "Male",
                IdNumber = "A123456789",
                Cellphone = "0912345678",
                Email = "test@example.com",
                Address = "Test Address",
                DateOfBirth = DateTime.Now.AddYears(-20)
            };

            var userRight = new UserRight
            {
                UserStatus = true,
                ShoppingPermission = true,
                MessagePermission = true,
                SalesAuthority = false
            };

            // Act
            var result = await _authService.RegisterUserAsync(user, userIntroduce, userRight);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Registration successful. Please confirm your email.", result.Message);
            Assert.NotNull(result.User);
        }

        [Fact]
        public async Task RegisterUserAsync_DuplicateAccount_ReturnsFailure()
        {
            // Arrange
            var existingUser = new User
            {
                UserAccount = "existinguser",
                UserName = "Existing User",
                UserPassword = "Password123!"
            };

            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserAccount = "existinguser",
                UserName = "Test User",
                UserPassword = "TestPassword123!"
            };

            var userIntroduce = new UserIntroduce
            {
                UserNickName = "TestNick",
                Gender = "Male",
                IdNumber = "A123456789",
                Cellphone = "0912345678",
                Email = "test@example.com",
                Address = "Test Address",
                DateOfBirth = DateTime.Now.AddYears(-20)
            };

            var userRight = new UserRight
            {
                UserStatus = true,
                ShoppingPermission = true,
                MessagePermission = true,
                SalesAuthority = false
            };

            // Act
            var result = await _authService.RegisterUserAsync(user, userIntroduce, userRight);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Account already exists.", result.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var user = new User
            {
                UserAccount = "testuser",
                UserName = "Test User",
                UserPassword = "TestPassword123!",
                UserEmailConfirmed = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authService.LoginUserAsync("testuser", "TestPassword123!");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Login successful.", result.Message);
            Assert.NotEmpty(result.Token);
        }

        [Fact]
        public async Task LoginUserAsync_InvalidCredentials_ReturnsFailure()
        {
            // Arrange
            var user = new User
            {
                UserAccount = "testuser",
                UserName = "Test User",
                UserPassword = "TestPassword123!",
                UserEmailConfirmed = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authService.LoginUserAsync("testuser", "WrongPassword");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid credentials.", result.Message);
        }

        [Fact]
        public async Task LockUserAccountAsync_ValidUser_ReturnsSuccess()
        {
            // Arrange
            var user = new User
            {
                UserAccount = "testuser",
                UserName = "Test User",
                UserPassword = "TestPassword123!"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authService.LockUserAccountAsync(user.UserId, TimeSpan.FromMinutes(15));

            // Assert
            Assert.True(result.Success);
            Assert.Equal("User account locked.", result.Message);
        }

        [Fact]
        public async Task UnlockUserAccountAsync_ValidUser_ReturnsSuccess()
        {
            // Arrange
            var user = new User
            {
                UserAccount = "testuser",
                UserName = "Test User",
                UserPassword = "TestPassword123!",
                UserLockoutEnabled = true,
                UserLockoutEnd = DateTime.UtcNow.AddMinutes(15)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authService.UnlockUserAccountAsync(user.UserId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("User account unlocked.", result.Message);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}