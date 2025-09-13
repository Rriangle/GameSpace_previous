using Microsoft.EntityFrameworkCore;
using GameSpace.Models;

namespace GameSpace.Data
{
    /// <summary>
    /// GameSpace 數據庫上下文
    /// </summary>
    public class GameSpaceDbContext : DbContext
    {
        public GameSpaceDbContext(DbContextOptions<GameSpaceDbContext> options) : base(options)
        {
        }

        // 用戶相關模型
        public DbSet<Users> Users { get; set; }
        public DbSet<UserIntroduce> UserIntroduces { get; set; }
        public DbSet<UserRight> UserRights { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<UserSignInStat> UserSignInStats { get; set; }
        public DbSet<MiniGame> MiniGames { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponType> CouponTypes { get; set; }
        public DbSet<EVoucher> EVouchers { get; set; }
        public DbSet<EVoucherType> EVoucherTypes { get; set; }
        public DbSet<WalletHistory> WalletHistories { get; set; }
        public DbSet<EVoucherToken> EVoucherTokens { get; set; }
        public DbSet<EVoucherRedeemLog> EVoucherRedeemLogs { get; set; }

        // 社群相關模型
        public DbSet<Post> Posts { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<ThreadPost> ThreadPosts { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }

        // 商務相關模型
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PlayerMarketProductInfo> PlayerMarketProducts { get; set; }
        public DbSet<PlayerMarketOrderInfo> PlayerMarketOrders { get; set; }

        // 遊戲相關模型
        public DbSet<Game> Games { get; set; }
        public DbSet<GameMetricDaily> GameMetricDailies { get; set; }
        public DbSet<GameProductDetail> GameProductDetails { get; set; }
        public DbSet<GameSourceMap> GameSourceMaps { get; set; }
        public DbSet<Forum> Forums { get; set; }

        // 錢包相關模型
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<WalletHistory> WalletHistories { get; set; }

        // 簽到相關模型
        public DbSet<DailyCheckIn> DailyCheckIns { get; set; }
        
        // 優惠券相關模型
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponType> CouponTypes { get; set; }
        
        // 電子禮券相關模型
        public DbSet<EVoucher> EVouchers { get; set; }
        public DbSet<EVoucherType> EVoucherTypes { get; set; }
        public DbSet<EVoucherRedeemLog> EVoucherRedeemLogs { get; set; }
        public DbSet<EVoucherToken> EVoucherTokens { get; set; }
        
        // 管理員相關模型
        public DbSet<ManagerData> ManagerData { get; set; }
        public DbSet<ManagerRole> ManagerRoles { get; set; }
        public DbSet<ManagerRolePermission> ManagerRolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置 Users 表
            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("User_ID");
                entity.Property(e => e.UserName).HasColumnName("User_Name");
                entity.Property(e => e.UserAccount).HasColumnName("User_Account");
                entity.Property(e => e.UserPassword).HasColumnName("User_Password");
                entity.Property(e => e.UserEmailConfirmed).HasColumnName("User_EmailConfirmed");
                entity.Property(e => e.UserPhoneNumberConfirmed).HasColumnName("User_PhoneNumberConfirmed");
                entity.Property(e => e.UserTwoFactorEnabled).HasColumnName("User_TwoFactorEnabled");
                entity.Property(e => e.UserAccessFailedCount).HasColumnName("User_AccessFailedCount");
                entity.Property(e => e.UserLockoutEnabled).HasColumnName("User_LockoutEnabled");
                entity.Property(e => e.UserLockoutEnd).HasColumnName("User_LockoutEnd");
            });
        }
    }
}