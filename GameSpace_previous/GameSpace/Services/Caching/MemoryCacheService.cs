using Microsoft.Extensions.Caching.Memory;

namespace GameSpace.Services.Caching
{
    /// <summary>
    /// 記憶體快取服務實現
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
                _logger.LogDebug("快取讀取: {Key} = {Found}", key, result != null ? "命中" : "未命中");
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "快取讀取失敗: {Key}", key);
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
                _logger.LogDebug("快取寫入: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "快取寫入失敗: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                _logger.LogDebug("快取移除: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "快取移除失敗: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            // 記憶體快取不支援模式匹配，這裡只記錄日誌
            _logger.LogWarning("記憶體快取不支援模式匹配移除: {Pattern}", pattern);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            try
            {
                var exists = _cache.TryGetValue(key, out _);
                _logger.LogDebug("快取檢查: {Key} = {Exists}", key, exists ? "存在" : "不存在");
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "快取檢查失敗: {Key}", key);
                return Task.FromResult(false);
            }
        }
    }
}
