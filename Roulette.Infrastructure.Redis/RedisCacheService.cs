using Roulette.Domain.Contracts.Redis;
using Roulette.Infrastructure.Redis.Settings;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Roulette.Infrastructure.Redis
{
    public class RedisCacheService(ConnectionMultiplexer connection, RedisSettings settings) : IRedisCacheService
    {
        private readonly IDatabase _db = connection.GetDatabase();
        private readonly RedisSettings _settings = settings;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task SetAsync<T>(string key, T value, int? ttlSeconds = null, bool overwrite = true)
        {
            ValidateKey(key);
            ValidateValue(value);

            var fullKey = FormatKey(key);
            if (!overwrite && await _db.KeyExistsAsync(fullKey).ConfigureAwait(false))
                return;

            var json = JsonSerializer.Serialize(value, _jsonOptions);
            var expiry = TimeSpan.FromSeconds(ttlSeconds ?? _settings.DefaultTtlSeconds);

            try
            {
                await _db.StringSetAsync(fullKey, json, expiry).ConfigureAwait(false);
            }
            catch (RedisConnectionException)
            {
                // Handle connection issues (e.g., log the error)
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            ValidateKey(key);

            var value = await _db.StringGetAsync(FormatKey(key)).ConfigureAwait(false);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions) : default;
        }

        public async Task<IEnumerable<T>?> GetListAsync<T>(string key)
        {
            ValidateKey(key);
            var fullKey = FormatKey(key);
            var value = await _db.StringGetAsync(fullKey).ConfigureAwait(false);
            return value.HasValue ? JsonSerializer.Deserialize<IEnumerable<T>>(value.ToString(), _jsonOptions) : default;
        }

        public async Task RemoveAsync(string key)
        {
            ValidateKey(key);

            await _db.KeyDeleteAsync(FormatKey(key)).ConfigureAwait(false);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            ValidateKey(key);

            return await _db.KeyExistsAsync(FormatKey(key)).ConfigureAwait(false);
        }

        private string FormatKey(string key) => $"{_settings.InstanceName}:{key}";

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));
        }

        private static void ValidateValue<T>(T value)
        {
            if (EqualityComparer<T?>.Default.Equals(value, default))
                throw new ArgumentNullException(nameof(value), "Cache value cannot be null.");
        }
    }
}
