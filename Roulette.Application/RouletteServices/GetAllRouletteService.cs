using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Entities;

namespace Roulette.Application.RouletteServices
{
    public class GetAllRouletteService(IRouletteRepository rouletteRepository)
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;

        public ListRouletteResponse Execute()
        {
            try
            {
                var roulettes = _rouletteRepository.GetAll();
                var rouletteResponses = roulettes?.Select(MapRouletteToResponse).ToList() ?? [];
                if (rouletteResponses.Count == 0)
                    return ListRouletteResponse.Fail("No roulettes found.");
                return ListRouletteResponse.Success(rouletteResponses);
            }
            catch (Exception ex)
            {
                return ListRouletteResponse.Fail($"Error retrieving roulettes: {ex.Message}");
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
