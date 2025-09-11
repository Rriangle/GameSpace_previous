using GameSpace.Core.Repositories;
using GameSpace.Core.Services;
using GameSpace.Core.Services.Seeding;
using GameSpace.Data;
using GameSpace.Infrastructure.Repositories;
using GameSpace.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace GameSpace.Infrastructure
{
    /// <summary>
    /// �̿�`�J�t�m�X�i
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// �K�[��¦�]�I�A��
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // �K�[��Ʈw�W�U��
            services.AddDbContext<GameSpaceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // �K�[�O����֨�
            services.AddMemoryCache();

            // �K�[�֤ߪA��
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddScoped<IRateLimitService, RateLimitService>();
            services.AddScoped<IRetryPolicyService, RetryPolicyService>();
            services.AddScoped<IRBACService, RBACService>();

            // �K�[�s�x�w
            services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();
            services.AddScoped<ISignInWriteRepository, SignInWriteRepository>();
            services.AddScoped<ICommunityReadOnlyRepository, CommunityReadOnlyRepository>();
            services.AddScoped<ICommerceReadOnlyRepository, CommerceReadOnlyRepository>();
            services.AddScoped<IForumReadOnlyRepository, ForumReadOnlyRepository>();
            services.AddScoped<ILeaderboardReadOnlyRepository, LeaderboardReadOnlyRepository>();
            services.AddScoped<IWalletReadOnlyRepository, WalletReadOnlyRepository>();

            // �K�[�ؤl�ƾڪA�� - �ɮɲ��ܨ��ѫ�y�D�D��
            // services.AddScoped<ISeedDataRunner, SeedDataRunner>();

            return services;
        }
    }
}
