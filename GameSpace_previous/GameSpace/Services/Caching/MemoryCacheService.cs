using Microsoft.Extensions.Caching.Memory;

namespace GameSpace.Services.Caching
{
    /// <summary>
    /// Memory cache service implementation
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var result = _cache.Get<T>(key);
                _logger.LogDebug("Cache read: {Key} = {Found}", key, result != null ? "hit" : "miss");
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache read failed: {Key}", key);
                return Task.FromResult<T?>(null);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = expiration ?? TimeSpan.FromMinutes(30)
                };
                
                _cache.Set(key, value, options);
                _logger.LogDebug("Cache write: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache write failed: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                _logger.LogDebug("Cache remove: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache remove failed: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            // Memory cache does not support pattern matching, only log here
            _logger.LogWarning("Memory cache does not support pattern matching removal: {Pattern}", pattern);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            try
            {
                var exists = _cache.TryGetValue(key, out _);
                _logger.LogDebug("Cache check: {Key} = {Exists}", key, exists ? "exists" : "not exists");
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache check failed: {Key}", key);
                return Task.FromResult(false);
            }
        }
    }
}
