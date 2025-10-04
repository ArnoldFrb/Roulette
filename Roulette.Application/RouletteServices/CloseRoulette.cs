using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Repositories.Base;
using Roulette.Domain.Contracts.Services;

namespace Roulette.Application.RouletteServices
{
    public class CloseRoulette(IRouletteRepository rouletteRepository, IBetRepository betRepository, IUnitOfWork unitOfWork)
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IBetRepository _betRepository = betRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public CloseRouletteResponse Execute(int rouletteId)
        {
            try
            {
                var roulette = _rouletteRepository.FindSingleOrDefault(r => r.Id == rouletteId);
                if (roulette == null)
                    return new CloseRouletteResponse(null, null, null, null, "Roulette not found.");

                var bets = _betRepository.FindBy(b => b.Roulette.Id == rouletteId);
                if (bets == null)
                    return new CloseRouletteResponse(roulette.Id, roulette.Status.ToString(), roulette.ClosedAt, null, "No bets found for this roulette.");

                roulette.CloseBet();
                _rouletteRepository.Edit(roulette);
                _unitOfWork.Commit();
                return new CloseRouletteResponse(roulette.Id, roulette.Status.ToString(), roulette.ClosedAt, bets, "Roulette closed successfully.");
            }
            catch (Exception ex)
            {
                return new CloseRouletteResponse(null, null, null, null, $"Error closing roulette: {ex.Message}");
            }
        }
    }
}
