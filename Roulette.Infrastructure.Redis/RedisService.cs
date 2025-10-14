using Microsoft.Extensions.DependencyInjection;
using Roulette.Domain.Contracts.Redis;
using Roulette.Infrastructure.Redis.Caches;
using Roulette.Infrastructure.Redis.Settings;
using StackExchange.Redis;

namespace Roulette.Infrastructure.Redis
{
    public static class RedisService
    {
        public static IServiceCollection AddRedisServices(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<RedisSettings>();

                int retry = 3;
                while (retry > 0)
                {
                    try
                    {
                        return ConnectionMultiplexer.Connect(settings.ConnectionString);
                    }
                    catch
                    {
                        retry--;
                        Thread.Sleep(1000);
                        if (retry == 0)
                            throw;
                    }
                }
                return null!;
            });

            services.AddSingleton<IRedisCacheService, RedisCacheService>();

            return services;
        }
    }
}
