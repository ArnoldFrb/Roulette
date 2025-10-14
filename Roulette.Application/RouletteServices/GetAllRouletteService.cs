using Microsoft.Extensions.Logging;
using Roulette.Application.Models;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services.Roulette;
using Roulette.Domain.Entities;
using static Roulette.Application.Models.AppCodes;

namespace Roulette.Application.RouletteServices
{
    public class GetAllRouletteService(IRouletteRepository rouletteRepository, IRedisCacheService redis, ILogger<GetAllRouletteService> logger) : IGetAllRouletteService<ListRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IRedisCacheService _redis = redis;
        private readonly ILogger<GetAllRouletteService> _logger = logger;

        public static readonly string CacheKey = "roulettes:all";

        public async Task<ListRouletteResponse> ExecuteAsync()
        {
            _logger.LogInformation("Attempt to obtain all roulette wheels");
            try
            {
                var cacheRoulettes = await _redis.GetListAsync<RouletteDto>(CacheKey);
                if (cacheRoulettes?.Any() == true)
                    return ListRouletteResponse.Success(cacheRoulettes);

                var roulettes = (await _rouletteRepository.GetAllAsync()).ToList() ?? [];

                if (roulettes.Count == 0)
                    return ListRouletteResponse.Success([], "No roulettes found.");

                if (roulettes.Count > 0)
                    await _redis.SetAsync(CacheKey, roulettes);

                var rouletteDto = roulettes.ConvertAll(MapRouletteToDto);

                return ListRouletteResponse.Success(rouletteDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error to obtain all roulette wheels");

                return ListRouletteResponse.Fail(AppCodes.System.INTERNAL_ERROR, ex.Message);
            }
        }

        private static DateTime GetRouletteDate(RouletteEntity roulette) =>
            roulette.Status switch
            {
                RouletteStatus.Created => roulette.CreatedAt,
                RouletteStatus.Open => roulette.OpenedAt,
                RouletteStatus.Closed => roulette.ClosedAt,
                _ => DateTime.MinValue
            };

        private static RouletteDto MapRouletteToDto(RouletteEntity roulette) =>
            new(
                roulette.Id,
                roulette.Status.ToString(),
                GetRouletteDate(roulette)
            );
    }
}
