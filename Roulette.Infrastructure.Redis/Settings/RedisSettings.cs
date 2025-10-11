using System.ComponentModel.DataAnnotations;

namespace Roulette.Infrastructure.Redis.Settings
{
    public class RedisSettings
    {
        [Required(ErrorMessage = "Redis:ConnectionString is required.")]
        public string ConnectionString { get; set; } = string.Empty;

        [Required]
        public string InstanceName { get; set; } = "roulette-api";

        [Range(1, 300, ErrorMessage = "DefaultTimeoutInSeconds must be between 1 and 300 seconds.")]
        public int DefaultTtlSeconds { get; set; } = 60;
    }
}
