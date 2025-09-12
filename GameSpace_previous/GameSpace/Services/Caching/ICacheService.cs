using System.Threading.Tasks;

namespace GameSpace.Services.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task<bool> ExistsAsync(string key);
        Task<long> IncrementAsync(string key, long value = 1);
        Task<long> DecrementAsync(string key, long value = 1);
        Task<Dictionary<string, T>> GetManyAsync<T>(IEnumerable<string> keys) where T : class;
        Task SetManyAsync<T>(Dictionary<string, T> values, TimeSpan? expiration = null) where T : class;
        Task FlushAllAsync();
        Task<Dictionary<string, object>> GetInfoAsync();
    }
}