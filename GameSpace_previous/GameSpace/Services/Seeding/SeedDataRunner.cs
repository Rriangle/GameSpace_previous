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

                // 創建用戶數據（200行）
                await CreateUserDataAsync(connection);
                
                // 創建遊戲數據（200行）
                await CreateGameDataAsync(connection);
                
                // 創建論壇數據（200行）
                await CreateForumDataAsync(connection);
                
                // 創建社群數據（200行）
                await CreateCommunityDataAsync(connection);
                
                // 創建商城數據（200行）
                await CreateCommerceDataAsync(connection);

                // 創建寵物數據（200行）
                await CreatePetDataAsync(connection);

                // 創建錢包數據（200行）
                await CreateWalletDataAsync(connection);

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
                
                return count < 200;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查是否需要種子數據時發生錯誤");
                return false;
            }
        }

        private async Task CreateUserDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建用戶數據（目標：200行）");

            // 檢查現有數據數量
            var countCommand = new SqlCommand("SELECT COUNT(*) FROM Users", connection);
            var existingCount = (int)await countCommand.ExecuteScalarAsync();
            
            if (existingCount >= 200)
            {
                _logger.LogInformation($"用戶數據已存在 {existingCount} 行，跳過創建");
                return;
            }

            var neededCount = 200 - existingCount;
            _logger.LogInformation($"需要創建 {neededCount} 行用戶數據");

            // 批量創建用戶數據
            for (int i = 0; i < neededCount; i += 1000) // 每批最多1000行
            {
                var batchSize = Math.Min(1000, neededCount - i);
                var userData = GenerateUserDataBatch(i + 1, batchSize);

                using var command = new SqlCommand(userData, connection);
                await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"已創建 {i + batchSize} 行用戶數據");
            }

            _logger.LogInformation("用戶數據創建完成");
        }

        private string GenerateUserDataBatch(int startId, int count)
        {
            var values = new List<string>();
            
            for (int i = 0; i < count; i++)
            {
                var id = startId + i;
                var account = $"user{id:D3}";
                var nickname = $"玩家{id}";
                var email = $"user{id}@gamespace.com";
                var phone = $"09{id:D8}";
                
                values.Add($"({id}, '{account}', 'Password001@', '{nickname}', '{email}', '{phone}', 'https://example.com/avatar{id}.jpg', 'Active', GETUTCDATE(), GETUTCDATE())");
            }

            return $@"
                INSERT INTO Users (User_ID, User_Account, User_Password, User_Name, User_Email, User_Phone, User_Avatar, User_Status, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n", values)}
            ";
        }

        private async Task CreateCommunityDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建社群數據（目標：200行）");

            // 檢查現有數據數量
            var countCommand = new SqlCommand("SELECT COUNT(*) FROM Forums", connection);
            var existingCount = (int)await countCommand.ExecuteScalarAsync();
            
            if (existingCount >= 200)
            {
                _logger.LogInformation($"論壇數據已存在 {existingCount} 行，跳過創建");
                return;
            }

            var neededCount = 200 - existingCount;
            _logger.LogInformation($"需要創建 {neededCount} 行論壇數據");

            // 批量創建論壇數據
            for (int i = 0; i < neededCount; i += 1000)
            {
                var batchSize = Math.Min(1000, neededCount - i);
                var forumData = GenerateForumDataBatch(i + 1, batchSize);

                using var command = new SqlCommand(forumData, connection);
                await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"已創建 {i + batchSize} 行論壇數據");
            }

            _logger.LogInformation("社群數據創建完成");
        }

        private string GenerateForumDataBatch(int startId, int count)
        {
            var values = new List<string>();
            var forumNames = new[] { "一般討論", "攻略分享", "新手教學", "活動公告", "交易市場", "技術支援", "遊戲心得", "閒聊區" };
            
            for (int i = 0; i < count; i++)
            {
                var id = startId + i;
                var forumName = $"{forumNames[id % forumNames.Length]}{id}";
                var description = $"論壇{id}的描述";
                
                values.Add($"({id}, 1, '{forumName}', '{description}', 0, 0, GETUTCDATE(), 1, GETUTCDATE(), GETUTCDATE())");
            }

            return $@"
                INSERT INTO Forums (Forum_ID, Game_ID, Forum_Name, Description, Thread_Count, Post_Count, Last_Activity, Is_Active, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n", values)}
            ";
        }

        private async Task CreateCommerceDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建商城數據（目標：200行）");

            // 檢查現有數據數量
            var countCommand = new SqlCommand("SELECT COUNT(*) FROM ProductInfo", connection);
            var existingCount = (int)await countCommand.ExecuteScalarAsync();
            
            if (existingCount >= 200)
            {
                _logger.LogInformation($"商品數據已存在 {existingCount} 行，跳過創建");
                return;
            }

            var neededCount = 200 - existingCount;
            _logger.LogInformation($"需要創建 {neededCount} 行商品數據");

            // 批量創建商品數據
            for (int i = 0; i < neededCount; i += 1000)
            {
                var batchSize = Math.Min(1000, neededCount - i);
                var productData = GenerateProductDataBatch(i + 1, batchSize);

                using var command = new SqlCommand(productData, connection);
                await command.ExecuteNonQueryAsync();
                
                _logger.LogInformation($"已創建 {i + batchSize} 行商品數據");
            }

            _logger.LogInformation("商城數據創建完成");
        }

        private string GenerateProductDataBatch(int startId, int count)
        {
            var values = new List<string>();
            var categories = new[] { "點數", "裝備", "道具", "禮包", "皮膚", "背景" };
            
            for (int i = 0; i < count; i++)
            {
                var id = startId + i;
                var productName = $"商品{id}";
                var description = $"商品{id}的描述";
                var price = 100 + (id % 1000);
                var category = categories[id % categories.Length];
                
                values.Add($"({id}, '{productName}', '{description}', {price}.00, 100, '{category}', 'https://example.com/product{id}.jpg', 1, GETUTCDATE(), GETUTCDATE())");
            }

            return $@"
                INSERT INTO ProductInfo (Product_ID, Product_Name, Description, Price, Stock, Category, Image_Url, Is_Active, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n", values)}
            ";
        }

        private async Task CreateAdditionalDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建其他必要數據（目標：200行）");

            // 創建寵物數據
            await CreatePetDataAsync(connection);
            
            // 創建優惠券數據
            await CreateCouponDataAsync(connection);
            
            // 創建電子禮券數據
            await CreateEVoucherDataAsync(connection);
        }

        private async Task CreatePetDataAsync(SqlConnection connection)
        {
            var countCommand = new SqlCommand("SELECT COUNT(*) FROM Pet", connection);
            var existingCount = (int)await countCommand.ExecuteScalarAsync();
            
            if (existingCount >= 200)
            {
                _logger.LogInformation($"寵物數據已存在 {existingCount} 行，跳過創建");
                return;
            }

            var neededCount = 200 - existingCount;
            _logger.LogInformation($"需要創建 {neededCount} 行寵物數據");

            for (int i = 0; i < neededCount; i += 1000)
            {
                var batchSize = Math.Min(1000, neededCount - i);
                var petData = GeneratePetDataBatch(i + 1, batchSize);

                using var command = new SqlCommand(petData, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("寵物數據創建完成");
        }

        private string GeneratePetDataBatch(int startId, int count)
        {
            var values = new List<string>();
            
            for (int i = 0; i < count; i++)
            {
                var id = startId + i;
                var userId = (id % 200) + 1; // 關聯到現有用戶
                var name = $"史萊姆{id}";
                var level = (id % 50) + 1;
                var hunger = (id % 100);
                var mood = (id % 100);
                var energy = (id % 100);
                var cleanliness = (id % 100);
                var health = (id % 100);
                
                values.Add($"({id}, {userId}, '{name}', {level}, {hunger}, {mood}, {energy}, {cleanliness}, {health}, 0, GETUTCDATE(), GETUTCDATE())");
            }

            return $@"
                INSERT INTO Pet (Pet_ID, User_ID, Pet_Name, Level, Hunger, Mood, Energy, Cleanliness, Health, Experience, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n", values)}
            ";
        }

        private async Task CreateCouponDataAsync(SqlConnection connection)
        {
            var countCommand = new SqlCommand("SELECT COUNT(*) FROM Coupon", connection);
            var existingCount = (int)await countCommand.ExecuteScalarAsync();
            
            if (existingCount >= 200)
            {
                _logger.LogInformation($"優惠券數據已存在 {existingCount} 行，跳過創建");
                return;
            }

            var neededCount = 200 - existingCount;
            _logger.LogInformation($"需要創建 {neededCount} 行優惠券數據");

            for (int i = 0; i < neededCount; i += 1000)
            {
                var batchSize = Math.Min(1000, neededCount - i);
                var couponData = GenerateCouponDataBatch(i + 1, batchSize);

                using var command = new SqlCommand(couponData, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("優惠券數據創建完成");
        }

        private string GenerateCouponDataBatch(int startId, int count)
        {
            var values = new List<string>();
            
            for (int i = 0; i < count; i++)
            {
                var id = startId + i;
                var userId = (id % 200) + 1;
                var code = $"COUPON{id:D6}";
                var discount = 10 + (id % 50);
                var isUsed = (id % 10) == 0; // 10% 已使用
                
                values.Add($"({id}, {userId}, '{code}', {discount}, {isUsed}, GETUTCDATE(), GETUTCDATE())");
            }

            return $@"
                INSERT INTO Coupon (Coupon_ID, User_ID, Coupon_Code, Discount_Amount, Is_Used, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n", values)}
            ";
        }

        private async Task CreateEVoucherDataAsync(SqlConnection connection)
        {
            var countCommand = new SqlCommand("SELECT COUNT(*) FROM EVoucher", connection);
            var existingCount = (int)await countCommand.ExecuteScalarAsync();
            
            if (existingCount >= 200)
            {
                _logger.LogInformation($"電子禮券數據已存在 {existingCount} 行，跳過創建");
                return;
            }

            var neededCount = 200 - existingCount;
            _logger.LogInformation($"需要創建 {neededCount} 行電子禮券數據");

            for (int i = 0; i < neededCount; i += 1000)
            {
                var batchSize = Math.Min(1000, neededCount - i);
                var evoucherData = GenerateEVoucherDataBatch(i + 1, batchSize);

                using var command = new SqlCommand(evoucherData, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("電子禮券數據創建完成");
        }

        private string GenerateEVoucherDataBatch(int startId, int count)
        {
            var values = new List<string>();
            
            for (int i = 0; i < count; i++)
            {
                var id = startId + i;
                var userId = (id % 200) + 1;
                var code = $"EVOUCHER{id:D6}";
                var value = 100 + (id % 500);
                var isUsed = (id % 20) == 0; // 5% 已使用
                
                values.Add($"({id}, {userId}, '{code}', {value}, {isUsed}, GETUTCDATE(), GETUTCDATE())");
            }

            return $@"
                INSERT INTO EVoucher (EVoucher_ID, User_ID, EVoucher_Code, Value, Is_Used, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n", values)}
            ";
        }

        private async Task CreateGameDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建遊戲數據 (目標: 200 行)");
            var existingCount = await GetTableRowCount(connection, "Games");
            if (existingCount >= 200)
            {
                _logger.LogInformation("Games 表已有 {Count} 行，無需新增。", existingCount);
                return;
            }

            var values = new List<string>();
            for (int i = existingCount + 1; i <= 200; i++)
            {
                var id = i;
                var gameName = $"遊戲_{i:D3}";
                var description = $"這是第 {i} 個遊戲的詳細描述，包含豐富的遊戲內容和特色功能。";
                var genre = new[] { "RPG", "動作", "策略", "射擊", "模擬", "競速", "益智" }[i % 7];
                var platform = new[] { "PC", "PS5", "Xbox", "Switch", "Mobile" }[i % 5];
                var releaseDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365));
                var imageUrl = $"https://example.com/game{id}.jpg";
                
                values.Add($"({id}, '{gameName}', '{description}', '{imageUrl}', '{genre}', '{platform}', '{releaseDate:yyyy-MM-dd}', 1, GETUTCDATE(), GETUTCDATE())");
            }

            if (values.Any())
            {
                var gameData = $"SET IDENTITY_INSERT Games ON; INSERT INTO Games (GameId, GameName, GameDescription, GameImageUrl, GameGenre, GamePlatform, ReleaseDate, IsActive, CreatedAt, UpdatedAt) VALUES {string.Join(",", values)}; SET IDENTITY_INSERT Games OFF;";
                using var command = new SqlCommand(gameData, connection);
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("已新增 {Count} 行遊戲數據。", values.Count);
            }
            _logger.LogInformation("遊戲數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "Games"));
        }

        private async Task CreateForumDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建論壇數據 (目標: 200 行)");
            var existingCount = await GetTableRowCount(connection, "Forums");
            if (existingCount >= 200)
            {
                _logger.LogInformation("Forums 表已有 {Count} 行，無需新增。", existingCount);
                return;
            }

            var values = new List<string>();
            for (int i = existingCount + 1; i <= 200; i++)
            {
                var id = i;
                var gameId = (i % 50) + 1; // 假設有50個遊戲
                var forumName = $"論壇_{i:D3}";
                var description = $"這是第 {i} 個論壇的詳細描述，討論相關遊戲內容。";
                var threadCount = Random.Shared.Next(10, 1000);
                var postCount = Random.Shared.Next(50, 5000);
                var lastActivity = DateTime.UtcNow.AddHours(-Random.Shared.Next(1, 168));
                
                values.Add($"({id}, {gameId}, '{forumName}', '{description}', {threadCount}, {postCount}, '{lastActivity:yyyy-MM-dd HH:mm:ss}', 1, GETUTCDATE(), GETUTCDATE())");
            }

            if (values.Any())
            {
                var forumData = $"SET IDENTITY_INSERT Forums ON; INSERT INTO Forums (ForumId, GameId, ForumName, Description, ThreadCount, PostCount, LastActivity, IsActive, CreatedAt, UpdatedAt) VALUES {string.Join(",", values)}; SET IDENTITY_INSERT Forums OFF;";
                using var command = new SqlCommand(forumData, connection);
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("已新增 {Count} 行論壇數據。", values.Count);
            }
            _logger.LogInformation("論壇數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "Forums"));
        }

        private async Task CreatePetDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建寵物數據 (目標: 200 行)");
            var existingCount = await GetTableRowCount(connection, "Pet");
            if (existingCount >= 200)
            {
                _logger.LogInformation("Pet 表已有 {Count} 行，無需新增。", existingCount);
                return;
            }

            var values = new List<string>();
            for (int i = existingCount + 1; i <= 200; i++)
            {
                var id = i;
                var userId = (i % 200) + 1; // 假設有200個用戶
                var petName = $"史萊姆_{i:D3}";
                var level = Random.Shared.Next(1, 50);
                var hunger = Random.Shared.Next(0, 100);
                var mood = Random.Shared.Next(0, 100);
                var energy = Random.Shared.Next(0, 100);
                var cleanliness = Random.Shared.Next(0, 100);
                var health = Random.Shared.Next(0, 100);
                var experience = Random.Shared.Next(0, 10000);
                
                values.Add($"({id}, {userId}, '{petName}', {level}, {hunger}, {mood}, {energy}, {cleanliness}, {health}, {experience}, GETUTCDATE(), GETUTCDATE())");
            }

            if (values.Any())
            {
                var petData = $"SET IDENTITY_INSERT Pet ON; INSERT INTO Pet (PetId, UserId, PetName, Level, Hunger, Mood, Energy, Cleanliness, Health, Experience, CreatedAt, UpdatedAt) VALUES {string.Join(",", values)}; SET IDENTITY_INSERT Pet OFF;";
                using var command = new SqlCommand(petData, connection);
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("已新增 {Count} 行寵物數據。", values.Count);
            }
            _logger.LogInformation("寵物數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "Pet"));
        }

        private async Task CreateWalletDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建錢包數據 (目標: 200 行)");
            var existingCount = await GetTableRowCount(connection, "User_Wallet");
            if (existingCount >= 200)
            {
                _logger.LogInformation("User_Wallet 表已有 {Count} 行，無需新增。", existingCount);
                return;
            }

            var values = new List<string>();
            for (int i = existingCount + 1; i <= 200; i++)
            {
                var userId = i;
                var points = Random.Shared.Next(0, 10000); // 0-10000點隨機餘額
                
                values.Add($"({userId}, {points}, GETUTCDATE(), GETUTCDATE())");
            }

            if (values.Any())
            {
                var walletData = $"INSERT INTO User_Wallet (UserId, UserPoint, CreatedAt, UpdatedAt) VALUES {string.Join(",", values)};";
                using var command = new SqlCommand(walletData, connection);
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("已新增 {Count} 行錢包數據。", values.Count);
            }
            _logger.LogInformation("錢包數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "User_Wallet"));
        }
    }
}