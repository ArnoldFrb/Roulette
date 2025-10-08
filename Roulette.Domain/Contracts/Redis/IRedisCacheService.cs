namespace Roulette.Domain.Contracts.Redis
{
    public interface IRedisCacheService
    {
        Task SetAsync<T>(string key, T value, int? ttlSeconds = null, bool overwrite = true);
        Task<T?> GetAsync<T>(string key);
        Task<IEnumerable<T>?> GetListAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
