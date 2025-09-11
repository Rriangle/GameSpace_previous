using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using GameSpace.Core.Services.Seeding;

namespace GameSpace.Services.Seeding
{
    /// <summary>
    /// 種子數據運行器實現
    /// </summary>
    public class SeedDataRunner : ISeedDataRunner
    {
        private readonly string _connectionString;
        private readonly ILogger<SeedDataRunner> _logger;

        public SeedDataRunner(IConfiguration configuration, ILogger<SeedDataRunner> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection");
            _logger = logger;
        }

        public async Task<bool> RunSeedDataAsync()
        {
            try
            {
                _logger.LogInformation("開始執行種子數據創建");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // 創建用戶數據
                await CreateUserDataAsync(connection);
                
                // 創建社群數據
                await CreateCommunityDataAsync(connection);
                
                // 創建商城數據
                await CreateCommerceDataAsync(connection);

                _logger.LogInformation("種子數據創建完成");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "種子數據創建失敗");
                return false;
            }
        }

        public async Task SeedAsync()
        {
            await RunSeedDataAsync();
        }

        public async Task<bool> NeedsSeedingAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new SqlCommand("SELECT COUNT(*) FROM Users", connection);
                var count = (int)await command.ExecuteScalarAsync();
                
                return count == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查是否需要種子數據時發生錯誤");
                return false;
            }
        }

        private async Task CreateUserDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建用戶數據");

            var userData = @"
                INSERT INTO Users (UserId, Account, Password, Nickname, Email, Phone, Avatar, Status, CreatedAt, UpdatedAt)
                VALUES 
                (1, 'admin', 'admin123', '管理員', 'admin@gamespace.com', '0912345678', 'https://example.com/avatar1.jpg', 'Active', GETUTCDATE(), GETUTCDATE()),
                (2, 'user1', 'user123', '玩家1', 'user1@gamespace.com', '0912345679', 'https://example.com/avatar2.jpg', 'Active', GETUTCDATE(), GETUTCDATE()),
                (3, 'user2', 'user123', '玩家2', 'user2@gamespace.com', '0912345680', 'https://example.com/avatar3.jpg', 'Active', GETUTCDATE(), GETUTCDATE())
            ";

            using var command = new SqlCommand(userData, connection);
            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("用戶數據創建完成");
        }

        private async Task CreateCommunityDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建社群數據");

            var communityData = @"
                INSERT INTO Forums (ForumId, GameId, ForumName, Description, ThreadCount, PostCount, LastActivity, IsActive, CreatedAt, UpdatedAt)
                VALUES 
                (1, 1, '一般討論', '遊戲一般討論區', 0, 0, GETUTCDATE(), 1, GETUTCDATE(), GETUTCDATE()),
                (2, 1, '攻略分享', '遊戲攻略分享區', 0, 0, GETUTCDATE(), 1, GETUTCDATE(), GETUTCDATE())
            ";

            using var command = new SqlCommand(communityData, connection);
            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("社群數據創建完成");
        }

        private async Task CreateCommerceDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建商城數據");

            var commerceData = @"
                INSERT INTO ProductInfo (ProductId, ProductName, Description, Price, Stock, Category, ImageUrl, IsActive, CreatedAt, UpdatedAt)
                VALUES 
                (1, '遊戲點數', '遊戲內點數', 100.00, 1000, '點數', 'https://example.com/points.jpg', 1, GETUTCDATE(), GETUTCDATE()),
                (2, '裝備包', '遊戲裝備包', 299.00, 100, '裝備', 'https://example.com/equipment.jpg', 1, GETUTCDATE(), GETUTCDATE())
            ";

            using var command = new SqlCommand(commerceData, connection);
            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("商城數據創建完成");
        }
    }
}
