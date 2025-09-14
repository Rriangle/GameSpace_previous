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
            // 創建每日簽到數據（200行）
            await CreateDailyCheckInDataAsync(connection);
            // 創建優惠券類型數據（200行）
            await CreateCouponTypeDataAsync(connection);
            // 創建優惠券數據（200行）
            await CreateCouponDataAsync(connection);
            // 創建電子禮券類型數據（200行）
            await CreateEVoucherTypeDataAsync(connection);
            // 創建電子禮券數據（200行）
            await CreateEVoucherDataAsync(connection);
            // 創建管理員數據（200行）
            await CreateManagerDataAsync(connection);
            // 創建通知數據（200行）
            await CreateNotificationDataAsync(connection);
            // 創建好友和群組數據（200行）
            await CreateFriendshipAndGroupDataAsync(connection);
            // 創建聊天數據（200行）
            await CreateChatDataAsync(connection);

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

        private async Task CreateDailyCheckInDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建每日簽到數據 (目標: 200 行)");
            
            var existingCount = await GetTableRowCount(connection, "DailyCheckIn");
            if (existingCount >= 200)
            {
                _logger.LogInformation("每日簽到數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42); // 固定種子確保可重複性

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var userId = random.Next(1, 201); // 假設有200個用戶
                var checkInDate = DateTime.Now.AddDays(-random.Next(0, 365)); // 過去一年內的隨機日期
                var consecutiveDays = random.Next(1, 31); // 1-30天連續簽到
                var rewardDetails = GenerateRewardDetails(consecutiveDays, random);
                var createdAt = checkInDate;
                var updatedAt = checkInDate;

                values.Add($"({i}, {userId}, '{checkInDate:yyyy-MM-dd HH:mm:ss}', {consecutiveDays}, '{rewardDetails}', '{createdAt:yyyy-MM-dd HH:mm:ss}', '{updatedAt:yyyy-MM-dd HH:mm:ss}')");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO DailyCheckIn (CheckInId, UserId, CheckInDate, ConsecutiveDays, RewardDetails, CreatedAt, UpdatedAt)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("每日簽到數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "DailyCheckIn"));
        }

        private string GenerateRewardDetails(int consecutiveDays, Random random)
        {
            var rewards = new List<string>();
            
            // 基礎點數獎勵
            var basePoints = 10 + Math.Min(consecutiveDays * 2, 50);
            rewards.Add($"點數: {basePoints}");
            
            // 寵物經驗獎勵
            var petExp = 5 + Math.Min(consecutiveDays, 20);
            rewards.Add($"寵物經驗: {petExp}");
            
            // 特殊獎勵（每7天）
            if (consecutiveDays % 7 == 0)
            {
                var specialRewards = new[] { "優惠券", "特殊道具", "額外點數", "寵物食物" };
                rewards.Add($"特殊獎勵: {specialRewards[random.Next(specialRewards.Length)]}");
            }
            
            // 連續簽到獎勵
            if (consecutiveDays >= 30)
            {
                rewards.Add("連續30天獎勵: 稀有道具");
            }
            else if (consecutiveDays >= 14)
            {
                rewards.Add("連續14天獎勵: 高級道具");
            }
            else if (consecutiveDays >= 7)
            {
                rewards.Add("連續7天獎勵: 普通道具");
            }
            
            return string.Join(", ", rewards);
        }

        private async Task CreateCouponTypeDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建優惠券類型數據 (目標: 200 行)");
            
            var existingCount = await GetTableRowCount(connection, "CouponType");
            if (existingCount >= 200)
            {
                _logger.LogInformation("優惠券類型數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42); // 固定種子確保可重複性

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var typeName = GenerateCouponTypeName(random);
                var description = GenerateCouponDescription(random);
                var discountType = random.Next(2) == 0 ? "Percentage" : "FixedAmount";
                var discountValue = discountType == "Percentage" ? random.Next(5, 51) : random.Next(10, 501);
                var minOrderAmount = random.Next(2) == 0 ? (decimal?)null : random.Next(100, 1001);
                var maxDiscountAmount = discountType == "Percentage" ? (decimal?)random.Next(50, 201) : null;
                var validFrom = DateTime.Now.AddDays(-random.Next(0, 30));
                var validTo = validFrom.AddDays(random.Next(30, 365));
                var createdAt = DateTime.Now.AddDays(-random.Next(0, 30));
                var updatedAt = createdAt;

                values.Add($"({i}, '{typeName}', '{description}', '{discountType}', {discountValue}, {minOrderAmount?.ToString() ?? "NULL"}, {maxDiscountAmount?.ToString() ?? "NULL"}, 1, '{validFrom:yyyy-MM-dd HH:mm:ss}', '{validTo:yyyy-MM-dd HH:mm:ss}', '{createdAt:yyyy-MM-dd HH:mm:ss}', '{updatedAt:yyyy-MM-dd HH:mm:ss}')");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO CouponType (CouponTypeID, TypeName, Description, DiscountType, DiscountValue, MinOrderAmount, MaxDiscountAmount, IsActive, ValidFrom, ValidTo, CreatedAt, UpdatedAt)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("優惠券類型數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "CouponType"));
        }

        private async Task CreateCouponDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建優惠券數據 (目標: 200 行)");
            
            var existingCount = await GetTableRowCount(connection, "Coupon");
            if (existingCount >= 200)
            {
                _logger.LogInformation("優惠券數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42); // 固定種子確保可重複性

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var couponCode = GenerateCouponCode(random);
                var couponTypeId = random.Next(1, 201); // 假設有200個優惠券類型
                var userId = random.Next(1, 201); // 假設有200個用戶
                var isUsed = random.Next(4) == 0; // 25%機率已使用
                var acquiredTime = DateTime.Now.AddDays(-random.Next(0, 90));
                var usedTime = isUsed ? (DateTime?)acquiredTime.AddDays(random.Next(1, 30)) : null;
                var usedInOrderId = isUsed ? (int?)random.Next(1, 1001) : null;

                values.Add($"({i}, '{couponCode}', {couponTypeId}, {userId}, {(isUsed ? 1 : 0)}, '{acquiredTime:yyyy-MM-dd HH:mm:ss}', {(usedTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")}, {(usedInOrderId?.ToString() ?? "NULL")})");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO Coupon (CouponID, CouponCode, CouponTypeID, UserID, IsUsed, AcquiredTime, UsedTime, UsedInOrderID)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("優惠券數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "Coupon"));
        }

        private async Task CreateEVoucherTypeDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建電子禮券類型數據 (目標: 200 行)");
            
            var existingCount = await GetTableRowCount(connection, "EVoucherType");
            if (existingCount >= 200)
            {
                _logger.LogInformation("電子禮券類型數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42); // 固定種子確保可重複性

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var typeName = GenerateEVoucherTypeName(random);
                var description = GenerateEVoucherDescription(random);
                var value = random.Next(50, 2001); // 50-2000元
                var currency = "TWD";
                var validFrom = DateTime.Now.AddDays(-random.Next(0, 30));
                var validTo = validFrom.AddDays(random.Next(30, 365));
                var createdAt = DateTime.Now.AddDays(-random.Next(0, 30));
                var updatedAt = createdAt;

                values.Add($"({i}, '{typeName}', '{description}', {value}, '{currency}', 1, '{validFrom:yyyy-MM-dd HH:mm:ss}', '{validTo:yyyy-MM-dd HH:mm:ss}', '{createdAt:yyyy-MM-dd HH:mm:ss}', '{updatedAt:yyyy-MM-dd HH:mm:ss}')");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO EVoucherType (EVoucherTypeID, TypeName, Description, Value, Currency, IsActive, ValidFrom, ValidTo, CreatedAt, UpdatedAt)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("電子禮券類型數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "EVoucherType"));
        }

        private async Task CreateEVoucherDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建電子禮券數據 (目標: 200 行)");
            
            var existingCount = await GetTableRowCount(connection, "EVoucher");
            if (existingCount >= 200)
            {
                _logger.LogInformation("電子禮券數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42); // 固定種子確保可重複性

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var eVoucherCode = GenerateEVoucherCode(random);
                var eVoucherTypeId = random.Next(1, 201); // 假設有200個電子禮券類型
                var userId = random.Next(1, 201); // 假設有200個用戶
                var isUsed = random.Next(4) == 0; // 25%機率已使用
                var acquiredTime = DateTime.Now.AddDays(-random.Next(0, 90));
                var usedTime = isUsed ? (DateTime?)acquiredTime.AddDays(random.Next(1, 30)) : null;
                var usedInOrderId = isUsed ? (int?)random.Next(1, 1001) : null;
                var expiryDate = acquiredTime.AddDays(30); // 30天後過期

                values.Add($"({i}, '{eVoucherCode}', {eVoucherTypeId}, {userId}, {(isUsed ? 1 : 0)}, '{acquiredTime:yyyy-MM-dd HH:mm:ss}', {(usedTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")}, {(usedInOrderId?.ToString() ?? "NULL")}, '{expiryDate:yyyy-MM-dd HH:mm:ss}')");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO EVoucher (EVoucherID, EVoucherCode, EVoucherTypeID, UserID, IsUsed, AcquiredTime, UsedTime, UsedInOrderID, ExpiryDate)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("電子禮券數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "EVoucher"));
        }

        private string GenerateCouponTypeName(Random random)
        {
            var prefixes = new[] { "新用戶", "會員專享", "限時", "節慶", "生日", "回饋", "推薦", "滿額" };
            var suffixes = new[] { "折扣券", "優惠券", "特價券", "免運券", "現金券" };
            return $"{prefixes[random.Next(prefixes.Length)]}{suffixes[random.Next(suffixes.Length)]}";
        }

        private string GenerateCouponDescription(Random random)
        {
            var descriptions = new[]
            {
                "限時優惠，數量有限",
                "新用戶專享優惠",
                "會員專屬優惠券",
                "節慶特別優惠",
                "生日專屬禮物",
                "推薦好友獎勵",
                "滿額贈送優惠券",
                "限時特價優惠"
            };
            return descriptions[random.Next(descriptions.Length)];
        }

        private string GenerateCouponCode(Random random)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"COUPON{result}";
        }

        private string GenerateEVoucherTypeName(Random random)
        {
            var prefixes = new[] { "現金", "購物", "美食", "娛樂", "旅遊", "數位", "實體", "通用" };
            var suffixes = new[] { "禮券", "代金券", "現金券", "購物券", "優惠券" };
            return $"{prefixes[random.Next(prefixes.Length)]}{suffixes[random.Next(suffixes.Length)]}";
        }

        private string GenerateEVoucherDescription(Random random)
        {
            var descriptions = new[]
            {
                "可在全站使用",
                "限特定商品使用",
                "無使用門檻",
                "限時優惠禮券",
                "會員專屬禮券",
                "節慶特別禮券",
                "生日專屬禮券",
                "推薦獎勵禮券"
            };
            return descriptions[random.Next(descriptions.Length)];
        }

        private string GenerateEVoucherCode(Random random)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"EV{result}";
        }

        private async Task CreateManagerDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建管理員數據 (目標: 200 行)");
            
            var existingCount = await GetTableRowCount(connection, "ManagerData");
            if (existingCount >= 200)
            {
                _logger.LogInformation("管理員數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42); // 固定種子確保可重複性

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var managerName = GenerateManagerName(random);
                var managerAccount = GenerateManagerAccount(random);
                var managerPassword = GenerateManagerPassword(random);
                var registrationDate = DateTime.Now.AddDays(-random.Next(0, 365));
                var managerEmail = GenerateManagerEmail(random);
                var emailConfirmed = random.Next(4) != 0; // 75%機率已確認
                var accessFailedCount = random.Next(0, 3);
                var lockoutEnabled = random.Next(10) == 0; // 10%機率啟用鎖定
                var lockoutEnd = lockoutEnabled && random.Next(2) == 0 ? 
                    (DateTime?)DateTime.Now.AddMinutes(-random.Next(1, 60)) : null;

                values.Add($"({i}, '{managerName}', '{managerAccount}', '{managerPassword}', '{registrationDate:yyyy-MM-dd HH:mm:ss}', '{managerEmail}', {(emailConfirmed ? 1 : 0)}, {accessFailedCount}, {(lockoutEnabled ? 1 : 0)}, {(lockoutEnd?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")})");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO ManagerData (Manager_Id, Manager_Name, Manager_Account, Manager_Password, Administrator_registration_date, Manager_Email, Manager_EmailConfirmed, Manager_AccessFailedCount, Manager_LockoutEnabled, Manager_LockoutEnd)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("管理員數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "ManagerData"));
        }

        private string GenerateManagerName(Random random)
        {
            var firstNames = new[] { "王", "李", "張", "劉", "陳", "楊", "黃", "趙", "周", "吳", "徐", "孫", "胡", "朱", "高", "林", "何", "郭", "馬", "羅" };
            var lastNames = new[] { "明", "華", "強", "偉", "軍", "勇", "磊", "濤", "超", "傑", "峰", "鵬", "斌", "輝", "亮", "剛", "健", "建", "文", "武" };
            return $"{firstNames[random.Next(firstNames.Length)]}{lastNames[random.Next(lastNames.Length)]}";
        }

        private string GenerateManagerAccount(Random random)
        {
            var prefixes = new[] { "admin", "manager", "supervisor", "director", "coordinator" };
            var numbers = random.Next(1000, 9999);
            return $"{prefixes[random.Next(prefixes.Length)]}{numbers}";
        }

        private string GenerateManagerPassword(Random random)
        {
            // 生成簡單的密碼哈希（實際應用中應使用更安全的方法）
            var password = $"Admin{random.Next(1000, 9999)}!";
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private string GenerateManagerEmail(Random random)
        {
            var domains = new[] { "gamespace.com", "admin.gamespace.com", "management.gamespace.com" };
            var username = GenerateManagerAccount(random);
            return $"{username}@{domains[random.Next(domains.Length)]}";
        }

        private async Task CreateNotificationDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建通知數據 (目標: 200 行)");
            
            // 先創建通知來源和動作
            await CreateNotificationSourcesAsync(connection);
            await CreateNotificationActionsAsync(connection);
            
            var existingCount = await GetTableRowCount(connection, "Notifications");
            if (existingCount >= 200)
            {
                _logger.LogInformation("通知數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42); // 固定種子確保可重複性

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var sourceId = random.Next(1, 11); // 假設有10個通知來源
                var actionId = random.Next(1, 11); // 假設有10個通知動作
                var title = GenerateNotificationTitle(random);
                var content = GenerateNotificationContent(random);
                var priority = new[] { "Low", "Normal", "High", "Urgent" }[random.Next(4)];
                var type = new[] { "Info", "Warning", "Error", "Success" }[random.Next(4)];
                var isRead = random.Next(3) == 0; // 33%機率已讀
                var createdAt = DateTime.Now.AddDays(-random.Next(0, 30));
                var readAt = isRead ? (DateTime?)createdAt.AddMinutes(random.Next(1, 1440)) : null;
                var expiresAt = random.Next(2) == 0 ? (DateTime?)createdAt.AddDays(random.Next(1, 30)) : null;

                values.Add($"({i}, {sourceId}, {actionId}, NULL, NULL, NULL, '{title}', '{content}', {(isRead ? 1 : 0)}, '{createdAt:yyyy-MM-dd HH:mm:ss}', {(readAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")}, {(expiresAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")}, '{priority}', '{type}')");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO Notifications (notification_id, source_id, action_id, group_id, sender_user_id, sender_manager_id, title, content, is_read, created_at, read_at, expires_at, priority, type)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            // 創建通知接收者記錄
            await CreateNotificationRecipientsAsync(connection);

            _logger.LogInformation("通知數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "Notifications"));
        }

        private async Task CreateNotificationSourcesAsync(SqlConnection connection)
        {
            var existingCount = await GetTableRowCount(connection, "Notification_Sources");
            if (existingCount >= 10)
            {
                return;
            }

            var sources = new[]
            {
                ("系統通知", "系統自動發送的通知"),
                ("活動通知", "遊戲活動相關通知"),
                ("訂單通知", "訂單狀態變更通知"),
                ("優惠券通知", "優惠券發放和使用通知"),
                ("安全通知", "帳號安全相關通知"),
                ("維護通知", "系統維護和更新通知"),
                ("社群通知", "社群互動相關通知"),
                ("推薦通知", "推薦和邀請通知"),
                ("客服通知", "客服支援相關通知"),
                ("公告通知", "重要公告和通知")
            };

            var values = new List<string>();
            for (int i = existingCount + 1; i <= 10; i++)
            {
                var source = sources[i - 1];
                var createdAt = DateTime.Now.AddDays(-random.Next(0, 30));
                values.Add($"({i}, '{source.Item1}', '{source.Item2}', 1, '{createdAt:yyyy-MM-dd HH:mm:ss}', '{createdAt:yyyy-MM-dd HH:mm:ss}')");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO Notification_Sources (source_id, source_name, description, is_active, created_at, updated_at)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task CreateNotificationActionsAsync(SqlConnection connection)
        {
            var existingCount = await GetTableRowCount(connection, "Notification_Actions");
            if (existingCount >= 10)
            {
                return;
            }

            var actions = new[]
            {
                ("查看詳情", "查看詳細信息", "/details", "Link"),
                ("立即處理", "立即處理相關事務", "/process", "Button"),
                ("前往頁面", "跳轉到相關頁面", "/redirect", "Redirect"),
                ("確認操作", "確認執行操作", "/confirm", "Modal"),
                ("忽略通知", "忽略此通知", "/ignore", "Button"),
                ("分享通知", "分享此通知", "/share", "Button"),
                ("回覆通知", "回覆此通知", "/reply", "Modal"),
                ("收藏通知", "收藏此通知", "/favorite", "Button"),
                ("舉報通知", "舉報此通知", "/report", "Modal"),
                ("關閉通知", "關閉此通知", "/close", "Button")
            };

            var values = new List<string>();
            for (int i = existingCount + 1; i <= 10; i++)
            {
                var action = actions[i - 1];
                var createdAt = DateTime.Now.AddDays(-random.Next(0, 30));
                values.Add($"({i}, '{action.Item1}', '{action.Item2}', '{action.Item3}', '{action.Item4}', 1, '{createdAt:yyyy-MM-dd HH:mm:ss}', '{createdAt:yyyy-MM-dd HH:mm:ss}')");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO Notification_Actions (action_id, action_name, description, action_url, action_type, is_active, created_at, updated_at)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task CreateNotificationRecipientsAsync(SqlConnection connection)
        {
            var existingCount = await GetTableRowCount(connection, "Notification_Recipients");
            if (existingCount >= 200)
            {
                return;
            }

            var values = new List<string>();
            var random = new Random(42);

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var notificationId = random.Next(1, 201); // 假設有200個通知
                var userId = random.Next(1, 201); // 假設有200個用戶
                var isRead = random.Next(3) == 0; // 33%機率已讀
                var createdAt = DateTime.Now.AddDays(-random.Next(0, 30));
                var readAt = isRead ? (DateTime?)createdAt.AddMinutes(random.Next(1, 1440)) : null;

                values.Add($"({i}, {notificationId}, {userId}, NULL, {(isRead ? 1 : 0)}, {(readAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")}, '{createdAt:yyyy-MM-dd HH:mm:ss}')");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO Notification_Recipients (recipient_id, notification_id, user_id, manager_id, is_read, read_at, created_at)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private string GenerateNotificationTitle(Random random)
        {
            var titles = new[]
            {
                "歡迎加入GameSpace！",
                "您有新的優惠券",
                "訂單狀態更新",
                "系統維護通知",
                "活動開始提醒",
                "帳號安全提醒",
                "好友邀請通知",
                "推薦獎勵到帳",
                "客服回覆通知",
                "重要公告發布"
            };
            return titles[random.Next(titles.Length)];
        }

        private string GenerateNotificationContent(Random random)
        {
            var contents = new[]
            {
                "感謝您註冊GameSpace，開始您的遊戲之旅吧！",
                "您獲得了一張新的優惠券，快來使用吧！",
                "您的訂單已發貨，請注意查收。",
                "系統將於今晚進行維護，請提前保存遊戲進度。",
                "限時活動已開始，快來參與贏取豐厚獎勵！",
                "檢測到異常登入，請確認是否為本人操作。",
                "您收到了一個好友邀請，快來查看吧！",
                "推薦好友成功，獎勵已發放到您的帳戶。",
                "您的問題已得到回覆，請查看詳細內容。",
                "重要公告已發布，請及時查看相關內容。"
            };
            return contents[random.Next(contents.Length)];
        }

        private async Task CreateFriendshipAndGroupDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建好友和群組數據 (目標: 200 行)");
            
            // 先創建群組數據
            await CreateGroupsDataAsync(connection);
            
            // 創建好友關係數據
            await CreateFriendshipDataAsync(connection);
            
            // 創建群組成員數據
            await CreateGroupMemberDataAsync(connection);
            
            _logger.LogInformation("好友和群組數據創建完成");
        }

        private async Task CreateGroupsDataAsync(SqlConnection connection)
        {
            var existingCount = await GetTableRowCount(connection, "Groups");
            if (existingCount >= 50)
            {
                _logger.LogInformation("群組數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42);

            for (int i = existingCount + 1; i <= 50; i++)
            {
                var ownerUserId = random.Next(1, 201); // 假設有200個用戶
                var groupName = GenerateGroupName(random);
                var description = GenerateGroupDescription(random);
                var isPrivate = random.Next(3) == 0; // 33%機率是私人群組
                var createdAt = DateTime.Now.AddDays(-random.Next(0, 90));
                var maxMembers = random.Next(10, 101); // 10-100人
                var groupAvatar = random.Next(2) == 0 ? $"avatar_{random.Next(1, 10)}.jpg" : null;

                values.Add($"({i}, {ownerUserId}, '{groupName}', '{description}', {(isPrivate ? 1 : 0)}, '{createdAt:yyyy-MM-dd HH:mm:ss}', NULL, {maxMembers}, {(groupAvatar != null ? $"'{groupAvatar}'" : "NULL")}, 1)");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO Groups (group_id, owner_user_id, group_name, description, is_private, created_at, updated_at, max_members, group_avatar, is_active)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("群組數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "Groups"));
        }

        private async Task CreateFriendshipDataAsync(SqlConnection connection)
        {
            var existingCount = await GetTableRowCount(connection, "Friendship");
            if (existingCount >= 200)
            {
                _logger.LogInformation("好友關係數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42);

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var userId = random.Next(1, 201); // 假設有200個用戶
                var friendId = random.Next(1, 201);
                
                // 確保不會自己加自己為好友
                if (userId == friendId)
                {
                    friendId = (friendId % 200) + 1;
                }
                
                var status = new[] { "Accepted", "Pending", "Blocked" }[random.Next(3)];
                var createdAt = DateTime.Now.AddDays(-random.Next(0, 90));
                var acceptedAt = status == "Accepted" ? (DateTime?)createdAt.AddDays(random.Next(1, 7)) : null;
                var blockedAt = status == "Blocked" ? (DateTime?)createdAt.AddDays(random.Next(1, 30)) : null;
                var note = random.Next(3) == 0 ? GenerateFriendshipNote(random) : null;

                values.Add($"({i}, {userId}, {friendId}, '{status}', '{createdAt:yyyy-MM-dd HH:mm:ss}', {(acceptedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")}, {(blockedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")}, {(note != null ? $"'{note}'" : "NULL")}, 1)");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO Friendship (friendship_id, user_id, friend_id, status, created_at, accepted_at, blocked_at, note, is_active)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("好友關係數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "Friendship"));
        }

        private async Task CreateGroupMemberDataAsync(SqlConnection connection)
        {
            var existingCount = await GetTableRowCount(connection, "Group_Member");
            if (existingCount >= 200)
            {
                _logger.LogInformation("群組成員數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42);

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var groupId = random.Next(1, 51); // 假設有50個群組
                var userId = random.Next(1, 201); // 假設有200個用戶
                var role = new[] { "Owner", "Admin", "Moderator", "Member" }[random.Next(4)];
                var joinedAt = DateTime.Now.AddDays(-random.Next(0, 90));
                var lastActiveAt = random.Next(2) == 0 ? (DateTime?)joinedAt.AddDays(random.Next(1, 30)) : null;
                var nickname = random.Next(3) == 0 ? GenerateNickname(random) : null;

                values.Add($"({i}, {groupId}, {userId}, '{role}', '{joinedAt:yyyy-MM-dd HH:mm:ss}', {(lastActiveAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")}, 1, {(nickname != null ? $"'{nickname}'" : "NULL")})");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO Group_Member (member_id, group_id, user_id, role, joined_at, last_active_at, is_active, nickname)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("群組成員數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "Group_Member"));
        }

        private string GenerateGroupName(Random random)
        {
            var prefixes = new[] { "遊戲", "聊天", "學習", "工作", "興趣", "運動", "音樂", "電影", "讀書", "旅行" };
            var suffixes = new[] { "群組", "小組", "團隊", "俱樂部", "社團", "聯盟", "公會", "家族" };
            return $"{prefixes[random.Next(prefixes.Length)]}{suffixes[random.Next(suffixes.Length)]}";
        }

        private string GenerateGroupDescription(Random random)
        {
            var descriptions = new[]
            {
                "歡迎加入我們的群組！",
                "一起分享遊戲心得",
                "學習交流的好地方",
                "工作討論專區",
                "興趣愛好分享",
                "運動健身群組",
                "音樂愛好者聚集地",
                "電影討論區",
                "讀書分享會",
                "旅行經驗交流"
            };
            return descriptions[random.Next(descriptions.Length)];
        }

        private string GenerateFriendshipNote(Random random)
        {
            var notes = new[]
            {
                "遊戲好友",
                "同學",
                "同事",
                "鄰居",
                "親戚",
                "網友",
                "興趣相投",
                "推薦好友",
                "活動認識",
                "共同朋友"
            };
            return notes[random.Next(notes.Length)];
        }

        private string GenerateNickname(Random random)
        {
            var nicknames = new[]
            {
                "小遊戲", "玩家", "高手", "新手", "老手", "達人", "專家", "愛好者", "粉絲", "朋友",
                "遊戲王", "大神", "菜鳥", "老鳥", "新星", "明星", "偶像", "偶像", "偶像", "偶像"
            };
            return nicknames[random.Next(nicknames.Length)];
        }

        private async Task CreateChatDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("創建聊天數據 (目標: 200 行)");
            
            // 先創建對話數據
            await CreateConversationsDataAsync(connection);
            
            // 創建消息數據
            await CreateMessagesDataAsync(connection);
            
            _logger.LogInformation("聊天數據創建完成");
        }

        private async Task CreateConversationsDataAsync(SqlConnection connection)
        {
            var existingCount = await GetTableRowCount(connection, "DM_Conversations");
            if (existingCount >= 100)
            {
                _logger.LogInformation("對話數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42);

            for (int i = existingCount + 1; i <= 100; i++)
            {
                var party1Id = random.Next(1, 201); // 假設有200個用戶
                var party2Id = random.Next(1, 201);
                
                // 確保不會自己和自己對話
                if (party1Id == party2Id)
                {
                    party2Id = (party2Id % 200) + 1;
                }
                
                var isManagerDm = random.Next(10) == 0; // 10%機率是管理員對話
                var createdAt = DateTime.Now.AddDays(-random.Next(0, 90));
                var lastMessageAt = random.Next(2) == 0 ? (DateTime?)createdAt.AddMinutes(random.Next(1, 1440)) : null;

                values.Add($"({i}, {(isManagerDm ? 1 : 0)}, {party1Id}, {party2Id}, '{createdAt:yyyy-MM-dd HH:mm:ss}', {(lastMessageAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")})");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO DM_Conversations (conversation_id, is_manager_dm, party1_id, party2_id, created_at, last_message_at)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("對話數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "DM_Conversations"));
        }

        private async Task CreateMessagesDataAsync(SqlConnection connection)
        {
            var existingCount = await GetTableRowCount(connection, "DM_Messages");
            if (existingCount >= 200)
            {
                _logger.LogInformation("消息數據已存在 {Count} 行，跳過創建", existingCount);
                return;
            }

            var values = new List<string>();
            var random = new Random(42);

            for (int i = existingCount + 1; i <= 200; i++)
            {
                var conversationId = random.Next(1, 101); // 假設有100個對話
                var senderUserId = random.Next(1, 201); // 假設有200個用戶
                var messageContent = GenerateMessageContent(random);
                var messageType = new[] { "Text", "Image", "File", "Voice", "Video" }[random.Next(5)];
                var sentAt = DateTime.Now.AddDays(-random.Next(0, 90)).AddMinutes(random.Next(0, 1440));
                var readAt = random.Next(3) == 0 ? (DateTime?)sentAt.AddMinutes(random.Next(1, 60)) : null;
                var isDeleted = random.Next(20) == 0; // 5%機率已刪除
                var deletedAt = isDeleted ? (DateTime?)sentAt.AddMinutes(random.Next(1, 60)) : null;
                var isEdited = random.Next(10) == 0; // 10%機率已編輯
                var editedAt = isEdited ? (DateTime?)sentAt.AddMinutes(random.Next(1, 30)) : null;
                var attachmentUrl = random.Next(5) == 0 ? $"attachment_{random.Next(1, 10)}.jpg" : null;
                var attachmentName = attachmentUrl != null ? $"附件_{random.Next(1, 100)}" : null;

                values.Add($"({i}, {conversationId}, {senderUserId}, '{messageContent}', '{messageType}', '{sentAt:yyyy-MM-dd HH:mm:ss}', {(readAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")}, {(isDeleted ? 1 : 0)}, {(deletedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")}, {(attachmentUrl != null ? $"'{attachmentUrl}'" : "NULL")}, {(attachmentName != null ? $"'{attachmentName}'" : "NULL")}, {(isEdited ? 1 : 0)}, {(editedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL")})");
            }

            if (values.Any())
            {
                var sql = $@"
                    INSERT INTO DM_Messages (message_id, conversation_id, sender_user_id, message_content, message_type, sent_at, read_at, is_deleted, deleted_at, attachment_url, attachment_name, is_edited, edited_at)
                    VALUES {string.Join(", ", values)}";

                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("消息數據創建完成，當前行數: {Count}", await GetTableRowCount(connection, "DM_Messages"));
        }

        private string GenerateMessageContent(Random random)
        {
            var messages = new[]
            {
                "你好！",
                "最近怎麼樣？",
                "在玩什麼遊戲？",
                "有空一起玩嗎？",
                "謝謝你的幫助！",
                "這個功能很好用",
                "我同意你的觀點",
                "明天見！",
                "晚安",
                "早上好",
                "今天天氣不錯",
                "你覺得這個怎麼樣？",
                "我明白了",
                "好的，沒問題",
                "太棒了！",
                "哈哈，有趣",
                "我也這麼想",
                "確實如此",
                "你說得對",
                "我不同意",
                "讓我考慮一下",
                "稍等一下",
                "我馬上回來",
                "抱歉，我遲到了",
                "沒關係",
                "不客氣",
                "再見！",
                "保持聯繫",
                "下次聊",
                "期待下次見面"
            };
            return messages[random.Next(messages.Length)];
        }
    }
}