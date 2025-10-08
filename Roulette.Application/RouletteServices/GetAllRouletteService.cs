using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services.Roulette;
using Roulette.Domain.Entities;

namespace Roulette.Application.RouletteServices
{
    public class GetAllRouletteService(IRouletteRepository rouletteRepository) : IGetAllRouletteService<ListRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;

        public async Task<ListRouletteResponse> ExecuteAsync()
        {
            try
            {
                var roulettes = await _rouletteRepository.GetAllAsync();

                var rouletteResponses = roulettes?.Select(MapRouletteToResponse).ToList() ?? [];
                if (rouletteResponses.Count == 0)
                    return ListRouletteResponse.Fail("No roulettes found.");

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
