using GameSpace.Core.Services;
using GameSpace.Data;
using GameSpace.Infrastructure.Repositories;
using GameSpace.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace GameSpace.Infrastructure
{
    /// <summary>
    /// 依賴注入配置擴展
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// 添加基礎設施服務
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 添加資料庫上下文
            services.AddDbContext<GameSpaceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 添加記憶體快取
            services.AddMemoryCache();

            // 添加核心服務
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddScoped<IRateLimitService, RateLimitService>();
            services.AddScoped<IRetryPolicyService, RetryPolicyService>();
            services.AddScoped<IRBACService, RBACService>();

            // 添加存儲庫
            services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();
            services.AddScoped<ISignInWriteRepository, SignInWriteRepository>();
            services.AddScoped<ICommunityReadOnlyRepository, CommunityReadOnlyRepository>();
            services.AddScoped<ICommerceReadOnlyRepository, CommerceReadOnlyRepository>();
            services.AddScoped<IForumReadOnlyRepository, ForumReadOnlyRepository>();
            services.AddScoped<ILeaderboardReadOnlyRepository, LeaderboardReadOnlyRepository>();
            services.AddScoped<IWalletReadOnlyRepository, WalletReadOnlyRepository>();

            // 添加種子數據服務
            services.AddScoped<ISeedDataRunner, SeedDataRunner>();

            return services;
        }
    }
}
