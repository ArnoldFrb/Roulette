using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

                var logger = sp.GetService<ILogger<IConnectionMultiplexer>>();

                var configOptions = ConfigurationOptions.Parse(settings.ConnectionString);
                configOptions.AbortOnConnectFail = false;
                configOptions.ConnectRetry = 3;
                configOptions.ConnectTimeout = 5000;
                configOptions.SyncTimeout = 5000;
                configOptions.ReconnectRetryPolicy = new ExponentialRetry(1000);

                const int maxRetries = 3;
                var baseDelay = TimeSpan.FromSeconds(1);

                for (int attempt = 0; attempt < maxRetries; attempt++)
                {
                    try
                    {
                        logger?.LogInformation("Attempting to connect to Redis (attempt {Attempt}/{MaxRetries})", attempt + 1, maxRetries);
                        return ConnectionMultiplexer.Connect(configOptions);
                    }
                    catch (Exception ex)
                    {
                        var isLastAttempt = attempt == maxRetries - 1;

                        if (isLastAttempt)
                        {
                            logger?.LogError(ex, "Failed to connect to Redis after {MaxRetries} attempts", maxRetries);
                            throw;
                        }

                        var delay = baseDelay * Math.Pow(2, attempt);
                        logger?.LogWarning(ex, "Redis connection attempt {Attempt} failed. Retrying in {Delay}ms", attempt + 1, delay.TotalMilliseconds);
                        Thread.Sleep(delay);
                    }
                }
                throw new InvalidOperationException("Failed to connect to Redis");
            });

            services.AddSingleton<IRedisCacheService, RedisCacheService>();

            return services;
        }
    }
}
