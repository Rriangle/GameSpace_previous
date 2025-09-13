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
        public DbSet<UserToken> UserTokens { get; set; }

        // 社群相關模型
        public DbSet<Post> Posts { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<ThreadPost> ThreadPosts { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }

        // 商務相關模型
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductInfo> ProductInfos { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderInfo> OrderInfos { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderAddress> OrderAddresses { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<PlayerMarketProductInfo> PlayerMarketProducts { get; set; }
        public DbSet<PlayerMarketOrderInfo> PlayerMarketOrders { get; set; }

        // 遊戲相關模型
        public DbSet<Game> Games { get; set; }
        public DbSet<GameMetricDaily> GameMetricDailies { get; set; }
        public DbSet<GameProductDetail> GameProductDetails { get; set; }
        public DbSet<GameSourceMap> GameSourceMaps { get; set; }

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
        
        // 錢包相關模型
        public DbSet<WalletHistory> WalletHistories { get; set; }
        
        // 管理員相關模型
        public DbSet<ManagerData> ManagerData { get; set; }
        public DbSet<ManagerRole> ManagerRoles { get; set; }
        public DbSet<ManagerRolePermission> ManagerRolePermissions { get; set; }
        
        // 通知相關模型
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationSource> NotificationSources { get; set; }
        public DbSet<NotificationAction> NotificationActions { get; set; }
        public DbSet<NotificationRecipient> NotificationRecipients { get; set; }
        
        // 好友和群組相關模型
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupBlock> GroupBlocks { get; set; }
        public DbSet<GroupReadState> GroupReadStates { get; set; }
        
        // 聊天相關模型
        public DbSet<DM_Conversations> DM_Conversations { get; set; }
        public DbSet<DM_Messages> DM_Messages { get; set; }
        
        // 供應商和服務提供商模型
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Provider> Providers { get; set; }
        
        // 客服工單模型
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<SupportTicketMessage> SupportTicketMessages { get; set; }
        public DbSet<SupportTicketAssignment> SupportTicketAssignments { get; set; }

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

            // 配置 UserWallet 表
            modelBuilder.Entity<UserWallet>(entity =>
            {
                entity.ToTable("User_Wallet");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("User_Id");
                entity.Property(e => e.UserPoint).HasColumnName("User_Point");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_At");
                
                // 添加約束：點數不能為負數
                entity.HasCheckConstraint("CK_UserWallet_UserPoint_NonNegative", "User_Point >= 0");
                
                // 添加約束：點數不能超過最大值
                entity.HasCheckConstraint("CK_UserWallet_UserPoint_MaxValue", "User_Point <= 999999999.99");
                
                // 配置與 Users 的關係
                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<UserWallet>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 配置 WalletHistory 表
            modelBuilder.Entity<WalletHistory>(entity =>
            {
                entity.ToTable("Wallet_History");
                entity.HasKey(e => e.HistoryId);
                entity.Property(e => e.HistoryId).HasColumnName("History_Id");
                entity.Property(e => e.UserId).HasColumnName("User_Id");
                entity.Property(e => e.TransactionType).HasColumnName("Transaction_Type");
                entity.Property(e => e.Amount).HasColumnName("Amount");
                entity.Property(e => e.BalanceBefore).HasColumnName("Balance_Before");
                entity.Property(e => e.BalanceAfter).HasColumnName("Balance_After");
                entity.Property(e => e.Description).HasColumnName("Description");
                entity.Property(e => e.ReferenceId).HasColumnName("Reference_Id");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                
                // 配置與 Users 的關係
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}