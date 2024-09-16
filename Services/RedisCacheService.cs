using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Server.Services
{
    public class RedisCacheService
    {
        /*private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCacheService(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _database.StringGetAsync(key);
            if (json.IsNullOrEmpty)
            {
                return default;
            }
            return System.Text.Json.JsonSerializer.Deserialize<T>(json!);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            var Server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = Server.Keys(pattern: pattern + "*").ToArray();
            if (keys.Length == 0)
            {
                return;
            }
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
        }*/

        private readonly IDistributedCache cache;
        private readonly ConnectionMultiplexer redis;

        public RedisCacheService(IDistributedCache cache,string connectionString)
        {
            this.cache = cache;
            this.redis = ConnectionMultiplexer.Connect(connectionString);
        }

        public void SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken token = default)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            },token);
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
        {
            var json = await cache.GetStringAsync(key, token);
            if (json == null)
            {
                return default;
            }
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            await cache.RemoveAsync(key, token);
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken token = default)
        {
            var server = redis.GetServer(redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern + "*").ToArray();
            if (keys.Length == 0)
            {
                return;
            }
            foreach (var key in keys)
            {
                await cache.RemoveAsync(key!, token);
            }
        }
    }
}
