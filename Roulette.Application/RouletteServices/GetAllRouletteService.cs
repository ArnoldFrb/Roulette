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
                var rouletteResponses = roulettes?.Select(MapRouletteToResponse) ?? [];
                if (!rouletteResponses.Any())
                    return ErrorResponse("No roulettes found.");
                return new ListRouletteResponse(rouletteResponses, "Roulettes retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Error retrieving roulettes: {ex.Message}");
            }
        }

        private static DateTime GetRouletteDate(RouletteEntity roulette) =>
            roulette.Status switch
            {
                BetStatus.Created => roulette.CreatedAt,
                BetStatus.Open => roulette.OpenedAt,
                BetStatus.Closed => roulette.ClosedAt,
                _ => DateTime.MinValue
            };

        private static RouletteResponse MapRouletteToResponse(RouletteEntity roulette) =>
            new(
                roulette.Id,
                roulette.Status.ToString(),
                GetRouletteDate(roulette)
            );

        private static ListRouletteResponse ErrorResponse(string message) =>
            new([], message);
    }
}
