using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;

namespace Roulette.Application.RouletteServices
{
    public class OpenRouletteService(IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork)
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public OpenRouletteResponse Execute(int rouletteId)
        {
            try
            {
                var roulette = _rouletteRepository.FindSingleOrDefault(r => r.Id == rouletteId);
                if (roulette == null)
                    return ErrorResponse("Roulette not found.");

                roulette.OpenBet();
                _rouletteRepository.Edit(roulette);
                _unitOfWork.Commit();

                return new OpenRouletteResponse(roulette.Id, roulette.Status.ToString(), roulette.OpenedAt, "Roulette opened successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Error opening roulette: {ex.Message}");
            }
        }

        private static OpenRouletteResponse ErrorResponse(string message) =>
            new(null, null, null, message);
    }
}
