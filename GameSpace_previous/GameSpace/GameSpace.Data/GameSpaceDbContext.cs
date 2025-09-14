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
        public DbSet<User> Users { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<UserSignInStats> UserSignInStats { get; set; }
        public DbSet<MiniGame> MiniGames { get; set; }

        // 優惠券和電子券相關模型
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponType> CouponTypes { get; set; }
        public DbSet<EVoucher> EVouchers { get; set; }
        public DbSet<EVoucherType> EVoucherTypes { get; set; }
        public DbSet<EVoucherToken> EVoucherTokens { get; set; }
        public DbSet<EVoucherRedeemLog> EVoucherRedeemLogs { get; set; }

        // 錢包歷史
        public DbSet<WalletHistory> WalletHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置 User 模型
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.ToTable("Users");
            });

            // 配置 UserWallet 模型
            modelBuilder.Entity<UserWallet>(entity =>
            {
                entity.HasKey(e => e.WalletID);
                entity.ToTable("UserWallets");
            });

            // 配置 Pet 模型
            modelBuilder.Entity<Pet>(entity =>
            {
                entity.HasKey(e => e.PetID);
                entity.ToTable("Pets");
            });

            // 配置 UserSignInStats 模型
            modelBuilder.Entity<UserSignInStats>(entity =>
            {
                entity.HasKey(e => e.StatID);
                entity.ToTable("UserSignInStats");
            });

            // 配置 MiniGame 模型
            modelBuilder.Entity<MiniGame>(entity =>
            {
                entity.HasKey(e => e.GameID);
                entity.ToTable("MiniGames");
            });

            // 配置 Coupon 模型
            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(e => e.CouponID);
                entity.ToTable("Coupons");
            });

            // 配置 CouponType 模型
            modelBuilder.Entity<CouponType>(entity =>
            {
                entity.HasKey(e => e.TypeID);
                entity.ToTable("CouponTypes");
            });

            // 配置 EVoucher 模型
            modelBuilder.Entity<EVoucher>(entity =>
            {
                entity.HasKey(e => e.VoucherID);
                entity.ToTable("EVouchers");
            });

            // 配置 EVoucherType 模型
            modelBuilder.Entity<EVoucherType>(entity =>
            {
                entity.HasKey(e => e.TypeID);
                entity.ToTable("EVoucherTypes");
            });

            // 配置 EVoucherToken 模型
            modelBuilder.Entity<EVoucherToken>(entity =>
            {
                entity.HasKey(e => e.TokenID);
                entity.ToTable("EVoucherTokens");
            });

            // 配置 EVoucherRedeemLog 模型
            modelBuilder.Entity<EVoucherRedeemLog>(entity =>
            {
                entity.HasKey(e => e.LogID);
                entity.ToTable("EVoucherRedeemLogs");
            });

            // 配置 WalletHistory 模型
            modelBuilder.Entity<WalletHistory>(entity =>
            {
                entity.HasKey(e => e.LogID);
                entity.ToTable("WalletHistories");
            });
        }
    }
}