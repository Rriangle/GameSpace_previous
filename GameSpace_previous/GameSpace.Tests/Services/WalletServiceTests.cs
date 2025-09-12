using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using GameSpace.Services.Wallet;
using GameSpace.Models;

namespace GameSpace.Tests.Services
{
    public class WalletServiceTests : IDisposable
    {
        private readonly GameSpacedatabaseContext _context;
        private readonly Mock<ILogger<WalletService>> _loggerMock;
        private readonly WalletService _walletService;

        public WalletServiceTests()
        {
            var options = new DbContextOptionsBuilder<GameSpacedatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameSpacedatabaseContext(options);
            _loggerMock = new Mock<ILogger<WalletService>>();
            _walletService = new WalletService(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task AddPointsAsync_ValidAmount_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var amount = 100;
            var description = "Test points";

            // Act
            var result = await _walletService.AddPointsAsync(userId, amount, description);

            // Assert
            Assert.True(result.Success);
            Assert.Equal($"Successfully added {amount} points.", result.Message);

            // Verify wallet was created
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            Assert.NotNull(wallet);
            Assert.Equal(amount, wallet.UserPoint);
        }

        [Fact]
        public async Task AddPointsAsync_NegativeAmount_ReturnsFailure()
        {
            // Arrange
            var userId = 1;
            var amount = -100;
            var description = "Test points";

            // Act
            var result = await _walletService.AddPointsAsync(userId, amount, description);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Points to add must be positive.", result.Message);
        }

        [Fact]
        public async Task DeductPointsAsync_SufficientBalance_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var wallet = new UserWallet
            {
                UserId = userId,
                UserPoint = 200
            };
            _context.UserWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            var result = await _walletService.DeductPointsAsync(userId, 100, "Test deduction");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Successfully deducted 100 points.", result.Message);

            // Verify balance was updated
            var updatedWallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            Assert.Equal(100, updatedWallet.UserPoint);
        }

        [Fact]
        public async Task DeductPointsAsync_InsufficientBalance_ReturnsFailure()
        {
            // Arrange
            var userId = 1;
            var wallet = new UserWallet
            {
                UserId = userId,
                UserPoint = 50
            };
            _context.UserWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            var result = await _walletService.DeductPointsAsync(userId, 100, "Test deduction");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Insufficient points.", result.Message);
        }

        [Fact]
        public async Task GetUserWalletAsync_ExistingWallet_ReturnsWallet()
        {
            // Arrange
            var userId = 1;
            var wallet = new UserWallet
            {
                UserId = userId,
                UserPoint = 100
            };
            _context.UserWallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            var result = await _walletService.GetUserWalletAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(100, result.UserPoint);
        }

        [Fact]
        public async Task GetUserWalletAsync_NonExistentWallet_ReturnsNull()
        {
            // Arrange
            var userId = 999;

            // Act
            var result = await _walletService.GetUserWalletAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetWalletHistoryAsync_ExistingHistory_ReturnsHistory()
        {
            // Arrange
            var userId = 1;
            var history1 = new WalletHistory
            {
                UserId = userId,
                ChangeType = "Point",
                PointsChanged = 100,
                Description = "Test 1",
                ChangeTime = DateTime.UtcNow.AddHours(-1)
            };
            var history2 = new WalletHistory
            {
                UserId = userId,
                ChangeType = "Point",
                PointsChanged = -50,
                Description = "Test 2",
                ChangeTime = DateTime.UtcNow
            };
            _context.WalletHistories.AddRange(history1, history2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _walletService.GetWalletHistoryAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(history2.ChangeTime, result.First().ChangeTime); // Should be ordered by ChangeTime desc
        }

        [Fact]
        public async Task RedeemCouponAsync_ValidCouponType_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            var wallet = new UserWallet
            {
                UserId = userId,
                UserPoint = 1000
            };
            _context.UserWallets.Add(wallet);

            var couponType = new CouponType
            {
                Name = "Test Coupon",
                Description = "Test Description",
                DiscountType = "Percentage",
                DiscountValue = 10,
                PointsCost = 100,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddDays(30)
            };
            _context.CouponTypes.Add(couponType);
            await _context.SaveChangesAsync();

            // Act
            var result = await _walletService.RedeemCouponAsync(userId, couponType.CouponTypeId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Coupon redeemed successfully.", result.Message);
            Assert.NotEmpty(result.CouponCode);

            // Verify coupon was created
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.UserId == userId);
            Assert.NotNull(coupon);
            Assert.Equal(couponType.CouponTypeId, coupon.CouponTypeId);
        }

        [Fact]
        public async Task RedeemCouponAsync_InsufficientPoints_ReturnsFailure()
        {
            // Arrange
            var userId = 1;
            var wallet = new UserWallet
            {
                UserId = userId,
                UserPoint = 50
            };
            _context.UserWallets.Add(wallet);

            var couponType = new CouponType
            {
                Name = "Test Coupon",
                Description = "Test Description",
                DiscountType = "Percentage",
                DiscountValue = 10,
                PointsCost = 100,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddDays(30)
            };
            _context.CouponTypes.Add(couponType);
            await _context.SaveChangesAsync();

            // Act
            var result = await _walletService.RedeemCouponAsync(userId, couponType.CouponTypeId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Insufficient points.", result.Message);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}