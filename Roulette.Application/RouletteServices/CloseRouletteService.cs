using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Contracts.Services.Roulette;
using Roulette.Domain.Entities;

namespace Roulette.Application.RouletteServices
{
    public class CloseRouletteService(IRouletteRepository rouletteRepository, IBetRepository betRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : ICloseRouletteService<CloseRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IBetRepository _betRepository = betRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public CloseRouletteResponse Execute(int rouletteId)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var roulette = _rouletteRepository.FindSingleOrDefault(r => r.Id == rouletteId);
                if (roulette == null)
                    return CloseRouletteResponse.Fail("Roulette not found.");

                var bets = _betRepository.FindBy(b => b.Roulette.Id == rouletteId).ToList() ?? [];

                roulette.CloseBet();
                roulette.GenerateWinningBet();

                ApplyBetResults(bets);

                foreach (var bet in bets)
                {
                    _userRepository.Edit(bet.User);
                    _betRepository.Edit(bet);
                }

                _rouletteRepository.Edit(roulette);
                _unitOfWork.CommitTransaction();

                var message = bets.Count != 0
                    ? "Roulette closed successfully."
                    : "Roulette closed successfully. No bets found for this roulette.";

                return CloseRouletteResponse.Success(
                    roulette.Id,
                    roulette.Status.ToString(),
                    roulette.CreatedAt, roulette.OpenedAt,
                    roulette.ClosedAt,
                    roulette.NumberWinner,
                    roulette.ColorWinner,
                    bets?.Select(MapBetToResponse).ToList(),
                    message);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return CloseRouletteResponse.Fail(ex.Message);
            }
        }

        private static void ApplyBetResults(IEnumerable<BetEntity> bets)
        {
            foreach (var bet in bets)
            {
                if (!bet.IsWinner()) continue;
                decimal payout = bet.GetWinnings();
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
    }
}
