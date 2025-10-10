using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services.Roulette;
using Roulette.Domain.Entities;

namespace Roulette.Application.RouletteServices
{
    public class GetAllRouletteService(IRouletteRepository rouletteRepository, IRedisCacheService redis) : IGetAllRouletteService<ListRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IRedisCacheService _redis = redis;

        public static readonly string CacheKey = "roulettes:all";

        public async Task<ListRouletteResponse> ExecuteAsync()
        {
            try
            {
                var cacheRoulettes = await _redis.GetListAsync<RouletteResponse>(CacheKey);
                if (cacheRoulettes?.Any() == true)
                    return ListRouletteResponse.Success(cacheRoulettes);

                var roulettes = await _rouletteRepository.GetAllAsync();

                var rouletteResponses = roulettes?.Select(MapRouletteToResponse).ToList() ?? [];
                if (rouletteResponses.Count == 0)
                    return ListRouletteResponse.Fail("No roulettes found.");

                await _redis.SetAsync(CacheKey, rouletteResponses);

                return ListRouletteResponse.Success(rouletteResponses);
            }
            catch (Exception ex)
            {
                return ListRouletteResponse.Fail(ex.Message);
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

        private static RouletteResponse MapRouletteToResponse(RouletteEntity roulette) =>
            new(
                roulette.Id,
                roulette.Status.ToString(),
                GetRouletteDate(roulette)
            );
    }
}
