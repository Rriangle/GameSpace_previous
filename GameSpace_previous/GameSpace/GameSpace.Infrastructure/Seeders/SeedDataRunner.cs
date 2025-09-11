using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Models;

namespace GameSpace.Infrastructure.Seeders
{
    public class SeedDataRunner
    {
        private readonly GameSpaceDbContext _context;

        public SeedDataRunner(GameSpaceDbContext context)
        {
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (await _context.Users.AnyAsync())
            {
                return; // Data already seeded
            }

            await SeedUsersAsync();
            await SeedUserWalletsAsync();
            await SeedUserSignInStatsAsync();
            await SeedWalletHistoryAsync();
            await SeedPetsAsync();
            await SeedMiniGamesAsync();
            await SeedCouponTypesAsync();
            await SeedCouponsAsync();
            await SeedEVoucherTypesAsync();
            await SeedEVouchersAsync();
            await SeedEVoucherTokensAsync();
            await SeedEVoucherRedeemLogsAsync();

            await _context.SaveChangesAsync();
        }

        private async Task SeedUsersAsync()
        {
            var users = new List<User>();
            var random = new Random();

            for (int i = 1; i <= 200; i++)
            {
                users.Add(new User
                {
                    UserID = 10000000 + i,
                    Username = $"user{i:D3}",
                    Email = $"user{i}@example.com",
                    Phone = $"09{i:D8}",
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 365)),
                    LastLoginAt = DateTime.Now.AddDays(-random.Next(0, 30)),
                    IsActive = random.Next(0, 10) < 8 // 80% active
                });
            }

            _context.Users.AddRange(users);
        }

        private async Task SeedUserWalletsAsync()
        {
            var wallets = new List<UserWallet>();
            var random = new Random();

            for (int i = 1; i <= 200; i++)
            {
                wallets.Add(new UserWallet
                {
                    WalletID = i,
                    UserID = 10000000 + i,
                    Points = random.Next(0, 1000),
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 365)),
                    UpdatedAt = DateTime.Now.AddDays(-random.Next(0, 30))
                });
            }

            _context.UserWallets.AddRange(wallets);
        }

        private async Task SeedUserSignInStatsAsync()
        {
            var stats = new List<UserSignInStats>();
            var random = new Random();

            for (int i = 1; i <= 200; i++)
            {
                var signInDate = DateTime.Now.AddDays(-random.Next(0, 30));
                stats.Add(new UserSignInStats
                {
                    StatID = i,
                    SignInDate = signInDate,
                    UserID = 10000000 + i,
                    PointsEarned = random.Next(5, 50),
                    CreatedAt = signInDate,
                    ConsecutiveDays = random.Next(1, 30),
                    UpdatedAt = signInDate,
                    Status = random.Next(0, 10) < 8 ? "1" : "0",
                    LastUpdated = signInDate
                });
            }

            _context.UserSignInStats.AddRange(stats);
        }

        private async Task SeedWalletHistoryAsync()
        {
            var history = new List<WalletHistory>();
            var random = new Random();
            var changeTypes = new[] { "Point", "Coupon", "EVoucher" };
            var descriptions = new[] { "遊戲獎勵點數", "訂單回饋點數", "每日簽到點數", "活動加碼點數", "兌換扣點", "獲得優惠券", "活動贈券", "訂單抵用券", "點數兌換禮券", "活動送禮券", "獲得禮券" };

            for (int i = 1; i <= 2000; i++)
            {
                var userId = 10000000 + random.Next(1, 201);
                var changeType = changeTypes[random.Next(changeTypes.Length)];
                var description = descriptions[random.Next(descriptions.Length)];

                history.Add(new WalletHistory
                {
                    LogID = i,
                    UserID = userId,
                    ChangeType = changeType,
                    PointsChanged = changeType == "Point" ? random.Next(-50, 51) : 0,
                    ItemCode = changeType != "Point" ? $"ITEM-{i:D6}" : null,
                    Description = description,
                    ChangeTime = DateTime.Now.AddDays(-random.Next(0, 365))
                });
            }

            _context.WalletHistory.AddRange(history);
        }

        private async Task SeedPetsAsync()
        {
            var pets = new List<Pet>();
            var random = new Random();
            var petTypes = new[] { "Dog", "Cat", "Bird", "Fish", "Rabbit" };

            for (int i = 1; i <= 200; i++)
            {
                pets.Add(new Pet
                {
                    PetID = i,
                    UserID = 10000000 + i,
                    PetName = $"Pet{i}",
                    PetType = petTypes[random.Next(petTypes.Length)],
                    Level = random.Next(1, 50),
                    Experience = random.Next(0, 1000),
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 365)),
                    UpdatedAt = DateTime.Now.AddDays(-random.Next(0, 30))
                });
            }

            _context.Pets.AddRange(pets);
        }

        private async Task SeedMiniGamesAsync()
        {
            var games = new List<MiniGame>();
            var random = new Random();
            var gameTypes = new[] { "Puzzle", "Action", "Strategy", "Arcade", "Racing" };

            for (int i = 1; i <= 50; i++)
            {
                games.Add(new MiniGame
                {
                    GameID = i,
                    GameName = $"Game{i}",
                    GameType = gameTypes[random.Next(gameTypes.Length)],
                    Description = $"Description for Game{i}",
                    PointsReward = random.Next(10, 100),
                    IsActive = random.Next(0, 10) < 8,
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 365)),
                    UpdatedAt = DateTime.Now.AddDays(-random.Next(0, 30))
                });
            }

            _context.MiniGames.AddRange(games);
        }

        private async Task SeedCouponTypesAsync()
        {
            var types = new List<CouponType>
            {
                new CouponType { TypeID = 1, TypeName = "折扣券", Description = "商品折扣優惠", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new CouponType { TypeID = 2, TypeName = "免運券", Description = "免運費優惠", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new CouponType { TypeID = 3, TypeName = "現金券", Description = "現金抵扣優惠", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };

            _context.CouponTypes.AddRange(types);
        }

        private async Task SeedCouponsAsync()
        {
            var coupons = new List<Coupon>();
            var random = new Random();

            for (int i = 1; i <= 100; i++)
            {
                coupons.Add(new Coupon
                {
                    CouponID = i,
                    TypeID = random.Next(1, 4),
                    CouponCode = $"CPN-{i:D6}",
                    CouponName = $"優惠券{i}",
                    Description = $"優惠券{i}描述",
                    DiscountValue = random.Next(10, 100),
                    DiscountType = "Percentage",
                    ExpiresAt = DateTime.Now.AddDays(random.Next(1, 365)),
                    IsActive = random.Next(0, 10) < 8,
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 365)),
                    UpdatedAt = DateTime.Now.AddDays(-random.Next(0, 30))
                });
            }

            _context.Coupons.AddRange(coupons);
        }

        private async Task SeedEVoucherTypesAsync()
        {
            var types = new List<EVoucherType>
            {
                new EVoucherType { TypeID = 1, TypeName = "電影票", Description = "電影票券", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new EVoucherType { TypeID = 2, TypeName = "咖啡券", Description = "咖啡店券", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new EVoucherType { TypeID = 3, TypeName = "美食券", Description = "餐廳券", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new EVoucherType { TypeID = 4, TypeName = "現金券", Description = "現金券", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new EVoucherType { TypeID = 5, TypeName = "商店券", Description = "商店券", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new EVoucherType { TypeID = 6, TypeName = "加油券", Description = "加油站券", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };

            _context.EVoucherTypes.AddRange(types);
        }

        private async Task SeedEVouchersAsync()
        {
            var eVouchers = new List<EVoucher>();
            var random = new Random();

            for (int i = 1; i <= 100; i++)
            {
                eVouchers.Add(new EVoucher
                {
                    VoucherID = i,
                    TypeID = random.Next(1, 7),
                    VoucherCode = $"EV-{i:D6}",
                    VoucherName = $"禮券{i}",
                    Description = $"禮券{i}描述",
                    Value = random.Next(50, 500),
                    ExpiresAt = DateTime.Now.AddDays(random.Next(1, 365)),
                    IsActive = random.Next(0, 10) < 8,
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 365)),
                    UpdatedAt = DateTime.Now.AddDays(-random.Next(0, 30))
                });
            }

            _context.EVouchers.AddRange(eVouchers);
        }

        private async Task SeedEVoucherTokensAsync()
        {
            var tokens = new List<EVoucherToken>();
            var random = new Random();

            for (int i = 1; i <= 200; i++)
            {
                tokens.Add(new EVoucherToken
                {
                    TokenID = i,
                    VoucherID = random.Next(1, 101),
                    UserID = 10000000 + random.Next(1, 201),
                    TokenCode = $"TOKEN-{i:D6}",
                    IsUsed = random.Next(0, 10) < 3,
                    ExpiresAt = DateTime.Now.AddDays(random.Next(1, 365)),
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 365)),
                    UpdatedAt = DateTime.Now.AddDays(-random.Next(0, 30))
                });
            }

            _context.EVoucherTokens.AddRange(tokens);
        }

        private async Task SeedEVoucherRedeemLogsAsync()
        {
            var logs = new List<EVoucherRedeemLog>();
            var random = new Random();

            for (int i = 1; i <= 100; i++)
            {
                logs.Add(new EVoucherRedeemLog
                {
                    LogID = i,
                    TokenID = random.Next(1, 201),
                    UserID = 10000000 + random.Next(1, 201),
                    RedeemedAt = DateTime.Now.AddDays(-random.Next(0, 365)),
                    Status = random.Next(0, 10) < 8 ? "Success" : "Failed",
                    CreatedAt = DateTime.Now.AddDays(-random.Next(0, 365))
                });
            }

            _context.EVoucherRedeemLogs.AddRange(logs);
        }
    }
}
