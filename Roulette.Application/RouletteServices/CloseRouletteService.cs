using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Repositories.Base;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Entities;

namespace Roulette.Application.RouletteServices
{
    public class CloseRouletteService(IRouletteRepository rouletteRepository, IBetRepository betRepository, IUnitOfWork unitOfWork)
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
                    return ErrorResponse("Roulette not found.");

                var bets = _betRepository.FindBy(b => b.Roulette.Id == rouletteId);

                roulette.CloseBet();
                roulette.GenerateWinningBet();

                ProcessBets(bets ?? []);

                _rouletteRepository.Edit(roulette);
                _unitOfWork.Commit();

                var betResponses = bets?.Select(MapBetToResponse).ToList();

                var message = !bets?.Any() ?? true ? "Roulette closed successfully. No bets found for this roulette." : "Roulette closed successfully.";

                return new CloseRouletteResponse(roulette.Id, roulette.Status.ToString(), roulette.ClosedAt, roulette.NumberWinner, roulette.ColorWinner, betResponses, message);
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Error closing roulette: {ex.Message}");
            }
        }

        private static void ProcessBets(IEnumerable<BetEntity> bets)
        {
            foreach (var bet in bets)
            {
                if (!bet.IsWinner()) continue;

                decimal payout = bet.BetType switch
                {
                    BetType.Number => bet.Amount * 5,
                    BetType.Color => bet.Amount * 1.8m,
                    _ => 0
                };

                bet.User.PayCredit(payout);
            }
        }

        private static BetResponse MapBetToResponse(BetEntity bet) =>
            new(
                bet.Id,
                bet.Amount,
                bet.BetType.ToString(),
                bet.BetType == BetType.Number ? bet.Number?.ToString() ?? "N/A" : bet.Color?.ToString() ?? "N/A",
                bet.IsWinner()
            );

        private static CloseRouletteResponse ErrorResponse(string message) =>
            new(null, null, null, null, null, null, message);
    }
}
