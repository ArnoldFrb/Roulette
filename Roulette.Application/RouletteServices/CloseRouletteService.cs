using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Contracts.Services.Roulette;
using Roulette.Domain.Entities;

namespace Roulette.Application.RouletteServices
{
    public class CloseRouletteService(IRouletteRepository rouletteRepository, IBetRepository betRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IRedisCacheService redis) : ICloseRouletteService<CloseRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IBetRepository _betRepository = betRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRedisCacheService _redis = redis;

        public async Task<CloseRouletteResponse> ExecuteAsync(int rouletteId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var roulette = await _rouletteRepository.FindSingleOrDefaultAsync(r => r.Id == rouletteId);
                if (roulette == null)
                    return CloseRouletteResponse.Fail("Roulette not found.");

                var resultBets = await _betRepository.FindByAsync(b => b.Roulette.Id == rouletteId) ?? [];
                var bets = resultBets.ToList();

                roulette.CloseBet();
                roulette.GenerateWinningBet();

                ApplyBetResults(bets);

                foreach (var bet in bets)
                {
                    await _userRepository.EditAsync(bet.User);
                    await _betRepository.EditAsync(bet);
                }

                await _rouletteRepository.EditAsync(roulette);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _redis.RemoveAsync(GetAllRouletteService.CacheKey);

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
                await _unitOfWork.RollbackTransactionAsync();
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
