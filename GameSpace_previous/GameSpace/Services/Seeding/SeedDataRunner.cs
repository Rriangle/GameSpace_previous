using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using GameSpace.Core.Services.Seeding;

namespace GameSpace.Services.Seeding
{
    /// <summary>
    /// Seed data runner implementation
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
                _logger.LogInformation("Starting seed data creation");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Create user data
                await CreateUserDataAsync(connection);
                
                // Create community data
                await CreateCommunityDataAsync(connection);
                
                // Create commerce data
                await CreateCommerceDataAsync(connection);

                _logger.LogInformation("Seed data creation completed");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Seed data creation failed");
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
                _logger.LogError(ex, "Error occurred while checking if seeding is needed");
                return false;
            }
        }

        private async Task CreateUserDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("Creating user data");

            // Check if users already exist
            var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Users", connection);
            var existingCount = (int)await checkCommand.ExecuteScalarAsync();
            
            if (existingCount >= 200)
            {
                _logger.LogInformation("Users table already has {Count} rows, skipping creation", existingCount);
                return;
            }

            // Create 200 users with realistic data
            var usersToCreate = 200 - existingCount;
            var userData = new List<string>();
            
            for (int i = existingCount + 1; i <= 200; i++)
            {
                var isAdmin = i == 1;
                var account = isAdmin ? "admin" : $"user{i}";
                var password = isAdmin ? "admin123" : $"user{i}123";
                var nickname = isAdmin ? "Administrator" : $"Player{i}";
                var email = isAdmin ? "admin@gamespace.com" : $"user{i}@gamespace.com";
                var phone = $"0912345{i:D4}";
                var avatar = $"https://example.com/avatar{i}.jpg";
                var status = i <= 190 ? "Active" : (i <= 195 ? "Inactive" : "Suspended");
                
                userData.Add($"({i}, '{account}', '{password}', '{nickname}', '{email}', '{phone}', '{avatar}', '{status}', GETUTCDATE(), GETUTCDATE())");
            }

            var insertSql = $@"
                INSERT INTO Users (UserId, Account, Password, Nickname, Email, Phone, Avatar, Status, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n", userData)}";

            using var command = new SqlCommand(insertSql, connection);
            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("User data creation completed - {Count} users created", usersToCreate);
        }

        private async Task CreateCommunityDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("Creating community data");

            // Check if forums already exist
            var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Forums", connection);
            var existingCount = (int)await checkCommand.ExecuteScalarAsync();
            
            if (existingCount >= 200)
            {
                _logger.LogInformation("Forums table already has {Count} rows, skipping creation", existingCount);
                return;
            }

            // Create 200 forums with realistic data
            var forumsToCreate = 200 - existingCount;
            var forumData = new List<string>();
            
            var gameTypes = new[] { "RPG", "FPS", "MOBA", "Strategy", "Puzzle", "Action", "Adventure", "Simulation", "Sports", "Racing" };
            var forumTypes = new[] { "General Discussion", "Strategy Sharing", "Bug Reports", "Feature Requests", "Community Events", "Guild Recruitment", "Trading", "Help & Support" };
            
            for (int i = existingCount + 1; i <= 200; i++)
            {
                var gameType = gameTypes[i % gameTypes.Length];
                var forumType = forumTypes[i % forumTypes.Length];
                var forumName = $"{gameType} - {forumType}";
                var description = $"Discussion area for {gameType} games - {forumType.ToLower()}";
                var threadCount = Random.Shared.Next(0, 100);
                var postCount = Random.Shared.Next(0, 1000);
                var isActive = i <= 190 ? 1 : 0;
                
                forumData.Add($"({i}, {i % 10 + 1}, '{forumName}', '{description}', {threadCount}, {postCount}, GETUTCDATE(), {isActive}, GETUTCDATE(), GETUTCDATE())");
            }

            var insertSql = $@"
                INSERT INTO Forums (ForumId, GameId, ForumName, Description, ThreadCount, PostCount, LastActivity, IsActive, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n", forumData)}";

            using var command = new SqlCommand(insertSql, connection);
            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("Community data creation completed - {Count} forums created", forumsToCreate);
        }

        private async Task CreateCommerceDataAsync(SqlConnection connection)
        {
            _logger.LogInformation("Creating commerce data");

            // Check if products already exist
            var checkCommand = new SqlCommand("SELECT COUNT(*) FROM ProductInfo", connection);
            var existingCount = (int)await checkCommand.ExecuteScalarAsync();
            
            if (existingCount >= 200)
            {
                _logger.LogInformation("ProductInfo table already has {Count} rows, skipping creation", existingCount);
                return;
            }

            // Create 200 products with realistic data
            var productsToCreate = 200 - existingCount;
            var productData = new List<string>();
            
            var categories = new[] { "Points", "Equipment", "Cosmetics", "Consumables", "Currency", "Boosters", "Pets", "Mounts", "Weapons", "Armor" };
            var productTypes = new[] { "Starter Pack", "Premium Pack", "Deluxe Edition", "Limited Edition", "Seasonal", "Event", "Bundle", "Single Item" };
            
            for (int i = existingCount + 1; i <= 200; i++)
            {
                var category = categories[i % categories.Length];
                var productType = productTypes[i % productTypes.Length];
                var productName = $"{category} {productType} #{i}";
                var description = $"High-quality {category.ToLower()} {productType.ToLower()} for enhanced gameplay experience";
                var price = Math.Round(Random.Shared.Next(50, 2000) + Random.Shared.NextDouble(), 2);
                var stock = Random.Shared.Next(10, 1000);
                var imageUrl = $"https://example.com/products/{category.ToLower()}_{i}.jpg";
                var isActive = i <= 190 ? 1 : 0;
                
                productData.Add($"({i}, '{productName}', '{description}', {price}, {stock}, '{category}', '{imageUrl}', {isActive}, GETUTCDATE(), GETUTCDATE())");
            }

            var insertSql = $@"
                INSERT INTO ProductInfo (ProductId, ProductName, Description, Price, Stock, Category, ImageUrl, IsActive, CreatedAt, UpdatedAt)
                VALUES {string.Join(",\n", productData)}";

            using var command = new SqlCommand(insertSql, connection);
            await command.ExecuteNonQueryAsync();

            _logger.LogInformation("Commerce data creation completed - {Count} products created", productsToCreate);
        }
    }
}
