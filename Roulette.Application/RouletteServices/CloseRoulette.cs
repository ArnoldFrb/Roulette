using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;

namespace Roulette.Application.RouletteServices
{
    public class CloseRoulette(IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork)
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public RouletteResponse Execute(int rouletteId)
        {
            try
            {
                var roulette = _rouletteRepository.FindSingleOrDefault(r => r.Id == rouletteId);
                if (roulette == null)
                    return new RouletteResponse(null, null, null, "Roulette not found.");
                roulette.CloseBet();
                _rouletteRepository.Edit(roulette);
                _unitOfWork.Commit();
                return new RouletteResponse(roulette.Id, roulette.Status.ToString(), roulette.ClosedAt, "Roulette closed successfully.");
            }
            catch (Exception ex)
            {
                return new RouletteResponse(null, null, null, $"Error closing roulette: {ex.Message}");
            }
        }
    }
}
