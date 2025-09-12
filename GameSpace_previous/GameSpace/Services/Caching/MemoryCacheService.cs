using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.Json;

namespace GameSpace.Services.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly ConcurrentDictionary<string, object> _keys = new();

        public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out var value))
                {
                    if (value is T directValue)
                    {
                        return Task.FromResult<T?>(directValue);
                    }

                    if (value is string jsonString)
                    {
                        var deserializedValue = JsonSerializer.Deserialize<T>(jsonString);
                        return Task.FromResult(deserializedValue);
                    }
                }

                return Task.FromResult<T?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cache value for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.Normal,
                    SlidingExpiration = TimeSpan.FromMinutes(30), // 默認30分鐘滑動過期
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1) // 默認1小時絕對過期
                };

                _memoryCache.Set(key, value, options);
                _keys.TryAdd(key, value);

                _logger.LogDebug("Cache set for key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set cache value for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _keys.TryRemove(key, out _);
                _logger.LogDebug("Cache removed for key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove cache value for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var keysToRemove = _keys.Keys
                    .Where(key => key.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _memoryCache.Remove(key);
                    _keys.TryRemove(key, out _);
                }

                _logger.LogDebug("Cache removed for pattern: {Pattern}, count: {Count}", pattern, keysToRemove.Count);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove cache values for pattern: {Pattern}", pattern);
                return Task.CompletedTask;
            }
        }

        public Task<bool> ExistsAsync(string key)
        {
            try
            {
                var exists = _memoryCache.TryGetValue(key, out _);
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check cache existence for key: {Key}", key);
                return Task.FromResult(false);
            }
        }

        public Task<long> IncrementAsync(string key, long value = 1)
        {
            try
            {
                var currentValue = _memoryCache.GetOrCreate(key, entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                    return 0L;
                });

                var newValue = currentValue + value;
                _memoryCache.Set(key, newValue, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });

                return Task.FromResult(newValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to increment cache value for key: {Key}", key);
                return Task.FromResult(0L);
            }
        }

        public Task<long> DecrementAsync(string key, long value = 1)
        {
            try
            {
                var currentValue = _memoryCache.GetOrCreate(key, entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                    return 0L;
                });

                var newValue = Math.Max(0, currentValue - value);
                _memoryCache.Set(key, newValue, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });

                return Task.FromResult(newValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decrement cache value for key: {Key}", key);
                return Task.FromResult(0L);
            }
        }

        public Task<Dictionary<string, T>> GetManyAsync<T>(IEnumerable<string> keys) where T : class
        {
            try
            {
                var result = new Dictionary<string, T>();

                foreach (var key in keys)
                {
                    if (_memoryCache.TryGetValue(key, out var value) && value is T typedValue)
                    {
                        result[key] = typedValue;
                    }
                }

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get many cache values");
                return Task.FromResult(new Dictionary<string, T>());
            }
        }

        public Task SetManyAsync<T>(Dictionary<string, T> values, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.Normal,
                    SlidingExpiration = TimeSpan.FromMinutes(30),
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
                };

                foreach (var kvp in values)
                {
                    _memoryCache.Set(kvp.Key, kvp.Value, options);
                    _keys.TryAdd(kvp.Key, kvp.Value);
                }

                _logger.LogDebug("Cache set for {Count} keys", values.Count);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set many cache values");
                return Task.CompletedTask;
            }
        }

        public Task FlushAllAsync()
        {
            try
            {
                if (_memoryCache is MemoryCache memCache)
                {
                    memCache.Compact(1.0); // 清理所有過期項目
                }

                _keys.Clear();
                _logger.LogInformation("Cache flushed");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to flush cache");
                return Task.CompletedTask;
            }
        }

        public Task<Dictionary<string, object>> GetInfoAsync()
        {
            try
            {
                var info = new Dictionary<string, object>
                {
                    ["TotalKeys"] = _keys.Count,
                    ["CacheType"] = "MemoryCache",
                    ["Keys"] = _keys.Keys.Take(100).ToList() // 只顯示前100個鍵
                };

                return Task.FromResult(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cache info");
                return Task.FromResult(new Dictionary<string, object>());
            }
        }
    }
}