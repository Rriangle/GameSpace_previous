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
        public DbSet<MetricSource> MetricSources { get; set; }
        public DbSet<LeaderboardSnapshot> LeaderboardSnapshots { get; set; }
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

        // 系統相關模型
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailQueue> EmailQueues { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<UserBlock> UserBlocks { get; set; }
        public DbSet<UserReport> UserReports { get; set; }
        
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

            // 配置 Game 表
            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("Games");
                entity.HasKey(e => e.GameId);
                entity.Property(e => e.GameId).HasColumnName("Game_Id");
                entity.Property(e => e.GameName).HasColumnName("Game_Name");
                entity.Property(e => e.GameDescription).HasColumnName("Game_Description");
                entity.Property(e => e.GameUrl).HasColumnName("Game_Url");
                entity.Property(e => e.GameImageUrl).HasColumnName("Game_ImageUrl");
                entity.Property(e => e.GameCategory).HasColumnName("Game_Category");
                entity.Property(e => e.GamePlatform).HasColumnName("Game_Platform");
                entity.Property(e => e.IsActive).HasColumnName("Is_Active");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_At");
            });

            // 配置 MetricSource 表
            modelBuilder.Entity<MetricSource>(entity =>
            {
                entity.ToTable("Metric_Sources");
                entity.HasKey(e => e.SourceId);
                entity.Property(e => e.SourceId).HasColumnName("Source_Id");
                entity.Property(e => e.SourceName).HasColumnName("Source_Name");
                entity.Property(e => e.SourceType).HasColumnName("Source_Type");
                entity.Property(e => e.ApiEndpoint).HasColumnName("Api_Endpoint");
                entity.Property(e => e.ApiKey).HasColumnName("Api_Key");
                entity.Property(e => e.Description).HasColumnName("Description");
                entity.Property(e => e.IsActive).HasColumnName("Is_Active");
                entity.Property(e => e.UpdateFrequency).HasColumnName("Update_Frequency");
                entity.Property(e => e.LastUpdated).HasColumnName("Last_Updated");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
            });

            // 配置 GameMetricDaily 表
            modelBuilder.Entity<GameMetricDaily>(entity =>
            {
                entity.ToTable("Game_Metric_Daily");
                entity.HasKey(e => e.MetricId);
                entity.Property(e => e.MetricId).HasColumnName("Metric_Id");
                entity.Property(e => e.GameId).HasColumnName("Game_Id");
                entity.Property(e => e.SourceId).HasColumnName("Source_Id");
                entity.Property(e => e.MetricDate).HasColumnName("Metric_Date");
                entity.Property(e => e.PlayerCount).HasColumnName("Player_Count");
                entity.Property(e => e.ViewCount).HasColumnName("View_Count");
                entity.Property(e => e.LikeCount).HasColumnName("Like_Count");
                entity.Property(e => e.ShareCount).HasColumnName("Share_Count");
                entity.Property(e => e.CommentCount).HasColumnName("Comment_Count");
                entity.Property(e => e.PlayTime).HasColumnName("Play_Time");
                entity.Property(e => e.Revenue).HasColumnName("Revenue");
                entity.Property(e => e.HeatScore).HasColumnName("Heat_Score");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_At");
                
                // 配置與 Game 的關係
                entity.HasOne(e => e.Game)
                    .WithMany(g => g.GameMetricDailies)
                    .HasForeignKey(e => e.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // 配置與 MetricSource 的關係
                entity.HasOne(e => e.Source)
                    .WithMany(s => s.GameMetricDailies)
                    .HasForeignKey(e => e.SourceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 配置 LeaderboardSnapshot 表
            modelBuilder.Entity<LeaderboardSnapshot>(entity =>
            {
                entity.ToTable("Leaderboard_Snapshots");
                entity.HasKey(e => e.SnapshotId);
                entity.Property(e => e.SnapshotId).HasColumnName("Snapshot_Id");
                entity.Property(e => e.GameId).HasColumnName("Game_Id");
                entity.Property(e => e.SnapshotType).HasColumnName("Snapshot_Type");
                entity.Property(e => e.Rank).HasColumnName("Rank");
                entity.Property(e => e.HeatScore).HasColumnName("Heat_Score");
                entity.Property(e => e.PlayerCount).HasColumnName("Player_Count");
                entity.Property(e => e.ViewCount).HasColumnName("View_Count");
                entity.Property(e => e.LikeCount).HasColumnName("Like_Count");
                entity.Property(e => e.ShareCount).HasColumnName("Share_Count");
                entity.Property(e => e.CommentCount).HasColumnName("Comment_Count");
                entity.Property(e => e.PlayTime).HasColumnName("Play_Time");
                entity.Property(e => e.Revenue).HasColumnName("Revenue");
                entity.Property(e => e.SnapshotDate).HasColumnName("Snapshot_Date");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                
                // 配置與 Game 的關係
                entity.HasOne(e => e.Game)
                    .WithMany(g => g.LeaderboardSnapshots)
                    .HasForeignKey(e => e.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 配置 SystemConfig 表
            modelBuilder.Entity<SystemConfig>(entity =>
            {
                entity.ToTable("System_Configs");
                entity.HasKey(e => e.ConfigId);
                entity.Property(e => e.ConfigId).HasColumnName("Config_Id");
                entity.Property(e => e.ConfigKey).HasColumnName("Config_Key");
                entity.Property(e => e.ConfigValue).HasColumnName("Config_Value");
                entity.Property(e => e.Description).HasColumnName("Description");
                entity.Property(e => e.ConfigType).HasColumnName("Config_Type");
                entity.Property(e => e.IsActive).HasColumnName("Is_Active");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_At");
            });

            // 配置 AuditLog 表
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("Audit_Logs");
                entity.HasKey(e => e.LogId);
                entity.Property(e => e.LogId).HasColumnName("Log_Id");
                entity.Property(e => e.UserId).HasColumnName("User_Id");
                entity.Property(e => e.Action).HasColumnName("Action");
                entity.Property(e => e.EntityType).HasColumnName("Entity_Type");
                entity.Property(e => e.EntityId).HasColumnName("Entity_Id");
                entity.Property(e => e.OldValues).HasColumnName("Old_Values");
                entity.Property(e => e.NewValues).HasColumnName("New_Values");
                entity.Property(e => e.IpAddress).HasColumnName("Ip_Address");
                entity.Property(e => e.UserAgent).HasColumnName("User_Agent");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // 配置 SystemLog 表
            modelBuilder.Entity<SystemLog>(entity =>
            {
                entity.ToTable("System_Logs");
                entity.HasKey(e => e.LogId);
                entity.Property(e => e.LogId).HasColumnName("Log_Id");
                entity.Property(e => e.LogLevel).HasColumnName("Log_Level");
                entity.Property(e => e.Message).HasColumnName("Message");
                entity.Property(e => e.Exception).HasColumnName("Exception");
                entity.Property(e => e.Source).HasColumnName("Source");
                entity.Property(e => e.UserId).HasColumnName("User_Id");
                entity.Property(e => e.IpAddress).HasColumnName("Ip_Address");
                entity.Property(e => e.UserAgent).HasColumnName("User_Agent");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // 配置 EmailTemplate 表
            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.ToTable("Email_Templates");
                entity.HasKey(e => e.TemplateId);
                entity.Property(e => e.TemplateId).HasColumnName("Template_Id");
                entity.Property(e => e.TemplateName).HasColumnName("Template_Name");
                entity.Property(e => e.Subject).HasColumnName("Subject");
                entity.Property(e => e.Body).HasColumnName("Body");
                entity.Property(e => e.TemplateType).HasColumnName("Template_Type");
                entity.Property(e => e.IsActive).HasColumnName("Is_Active");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_At");
            });

            // 配置 EmailQueue 表
            modelBuilder.Entity<EmailQueue>(entity =>
            {
                entity.ToTable("Email_Queues");
                entity.HasKey(e => e.QueueId);
                entity.Property(e => e.QueueId).HasColumnName("Queue_Id");
                entity.Property(e => e.UserId).HasColumnName("User_Id");
                entity.Property(e => e.ToEmail).HasColumnName("To_Email");
                entity.Property(e => e.Subject).HasColumnName("Subject");
                entity.Property(e => e.Body).HasColumnName("Body");
                entity.Property(e => e.Status).HasColumnName("Status");
                entity.Property(e => e.TemplateId).HasColumnName("Template_Id");
                entity.Property(e => e.RetryCount).HasColumnName("Retry_Count");
                entity.Property(e => e.SentAt).HasColumnName("Sent_At");
                entity.Property(e => e.ErrorMessage).HasColumnName("Error_Message");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                entity.Property(e => e.ScheduledAt).HasColumnName("Scheduled_At");
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(e => e.Template)
                    .WithMany()
                    .HasForeignKey(e => e.TemplateId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // 配置 FileUpload 表
            modelBuilder.Entity<FileUpload>(entity =>
            {
                entity.ToTable("File_Uploads");
                entity.HasKey(e => e.FileId);
                entity.Property(e => e.FileId).HasColumnName("File_Id");
                entity.Property(e => e.UserId).HasColumnName("User_Id");
                entity.Property(e => e.FileName).HasColumnName("File_Name");
                entity.Property(e => e.OriginalFileName).HasColumnName("Original_File_Name");
                entity.Property(e => e.FilePath).HasColumnName("File_Path");
                entity.Property(e => e.FileType).HasColumnName("File_Type");
                entity.Property(e => e.FileSize).HasColumnName("File_Size");
                entity.Property(e => e.Description).HasColumnName("Description");
                entity.Property(e => e.UploadType).HasColumnName("Upload_Type");
                entity.Property(e => e.IsPublic).HasColumnName("Is_Public");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // 配置 UserActivity 表
            modelBuilder.Entity<UserActivity>(entity =>
            {
                entity.ToTable("User_Activities");
                entity.HasKey(e => e.ActivityId);
                entity.Property(e => e.ActivityId).HasColumnName("Activity_Id");
                entity.Property(e => e.UserId).HasColumnName("User_Id");
                entity.Property(e => e.ActivityType).HasColumnName("Activity_Type");
                entity.Property(e => e.Description).HasColumnName("Description");
                entity.Property(e => e.EntityType).HasColumnName("Entity_Type");
                entity.Property(e => e.EntityId).HasColumnName("Entity_Id");
                entity.Property(e => e.IpAddress).HasColumnName("Ip_Address");
                entity.Property(e => e.UserAgent).HasColumnName("User_Agent");
                entity.Property(e => e.Location).HasColumnName("Location");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 配置 UserSession 表
            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.ToTable("User_Sessions");
                entity.HasKey(e => e.SessionId);
                entity.Property(e => e.SessionId).HasColumnName("Session_Id");
                entity.Property(e => e.UserId).HasColumnName("User_Id");
                entity.Property(e => e.SessionToken).HasColumnName("Session_Token");
                entity.Property(e => e.DeviceInfo).HasColumnName("Device_Info");
                entity.Property(e => e.IpAddress).HasColumnName("Ip_Address");
                entity.Property(e => e.Location).HasColumnName("Location");
                entity.Property(e => e.IsActive).HasColumnName("Is_Active");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                entity.Property(e => e.LastActivity).HasColumnName("Last_Activity");
                entity.Property(e => e.ExpiresAt).HasColumnName("Expires_At");
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 配置 UserPreference 表
            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.ToTable("User_Preferences");
                entity.HasKey(e => e.PreferenceId);
                entity.Property(e => e.PreferenceId).HasColumnName("Preference_Id");
                entity.Property(e => e.UserId).HasColumnName("User_Id");
                entity.Property(e => e.PreferenceKey).HasColumnName("Preference_Key");
                entity.Property(e => e.PreferenceValue).HasColumnName("Preference_Value");
                entity.Property(e => e.Description).HasColumnName("Description");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_At");
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 配置 UserBlock 表
            modelBuilder.Entity<UserBlock>(entity =>
            {
                entity.ToTable("User_Blocks");
                entity.HasKey(e => e.BlockId);
                entity.Property(e => e.BlockId).HasColumnName("Block_Id");
                entity.Property(e => e.BlockerId).HasColumnName("Blocker_Id");
                entity.Property(e => e.BlockedId).HasColumnName("Blocked_Id");
                entity.Property(e => e.Reason).HasColumnName("Reason");
                entity.Property(e => e.IsActive).HasColumnName("Is_Active");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                entity.Property(e => e.UnblockedAt).HasColumnName("Unblocked_At");
                
                entity.HasOne(e => e.Blocker)
                    .WithMany()
                    .HasForeignKey(e => e.BlockerId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Blocked)
                    .WithMany()
                    .HasForeignKey(e => e.BlockedId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 配置 UserReport 表
            modelBuilder.Entity<UserReport>(entity =>
            {
                entity.ToTable("User_Reports");
                entity.HasKey(e => e.ReportId);
                entity.Property(e => e.ReportId).HasColumnName("Report_Id");
                entity.Property(e => e.ReporterId).HasColumnName("Reporter_Id");
                entity.Property(e => e.ReportedUserId).HasColumnName("Reported_User_Id");
                entity.Property(e => e.ReportedEntityType).HasColumnName("Reported_Entity_Type");
                entity.Property(e => e.ReportedEntityId).HasColumnName("Reported_Entity_Id");
                entity.Property(e => e.ReportType).HasColumnName("Report_Type");
                entity.Property(e => e.Description).HasColumnName("Description");
                entity.Property(e => e.Status).HasColumnName("Status");
                entity.Property(e => e.ReviewedBy).HasColumnName("Reviewed_By");
                entity.Property(e => e.ReviewNotes).HasColumnName("Review_Notes");
                entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
                entity.Property(e => e.ReviewedAt).HasColumnName("Reviewed_At");
                
                entity.HasOne(e => e.Reporter)
                    .WithMany()
                    .HasForeignKey(e => e.ReporterId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.ReportedUser)
                    .WithMany()
                    .HasForeignKey(e => e.ReportedUserId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(e => e.Reviewer)
                    .WithMany()
                    .HasForeignKey(e => e.ReviewedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}