using Microsoft.Extensions.Logging;
using Roulette.Application.Models;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Contracts.Services.Roulette;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Application.RouletteServices
{
    public class CloseRouletteService(
        IRouletteRepository rouletteRepository,
        IBetRepository betRepository, IGamblerRepository gamblerRepository,
        IUnitOfWork unitOfWork,
        IRedisCacheService redis,
        ILogger<CloseRouletteService> logger
        ) : ICloseRouletteService<CloseRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IGamblerRepository _gamblerRepository = gamblerRepository;
        private readonly IBetRepository _betRepository = betRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRedisCacheService _redis = redis;
        private readonly ILogger<CloseRouletteService> _logger = logger;

        public async Task<CloseRouletteResponse> ExecuteAsync(int rouletteId)
        {
            _logger.LogInformation("Attempt to open a roulette");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var roulette = await _rouletteRepository.FindSingleOrDefaultAsync(r => r.Id == rouletteId);
                if (roulette is null)
                    return CloseRouletteResponse.Fail(AppCodes.Roulette.ROULETTE_NOT_FOUND, "Roulette not found.");

                roulette.CloseRoulette();

                var bets = (await _betRepository.FindByAsync(b => b.RouletteId == rouletteId)).ToList() ?? [];

                await ProcessBetsAsync(bets, roulette);

                await _rouletteRepository.EditAsync(roulette);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _redis.RemoveAsync(GetAllRouletteService.CacheKey);

                var message = bets.Count == 0
                    ? "Roulette closed successfully. No bets found for this roulette."
                    : "Roulette closed successfully.";

                return CloseRouletteResponse.Success(new CloseRouletteDto()
                {
                    Id = roulette.Id,
                    Status = roulette.Status.ToString(),
                    CreatedAt = roulette.CreatedAt,
                    OpenedAt = roulette.OpenedAt,
                    ClosedAt = roulette.ClosedAt,
                    NumberWinner = roulette.NumberWinner,
                    ColorWinner = roulette.ColorWinner,
                    Bets = bets.ConvertAll(MapBetToResponse)
                }, message);
            }
            catch (InvalidRouletteStatusException ex)
            {
                _logger.LogError(ex, "Attempt to close a roulette {RouletteId}", rouletteId);

                await _unitOfWork.RollbackTransactionAsync();
                return CloseRouletteResponse.Fail(AppCodes.Roulette.ROULETTE_CLOSE_ERROR, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Attempt to close a roulette {RouletteId}", rouletteId);
                await _unitOfWork.RollbackTransactionAsync();
                return CloseRouletteResponse.Fail(AppCodes.System.INTERNAL_ERROR, ex.Message);
            }
        }

        private async Task ProcessBetsAsync(List<BetEntity> bets, RouletteEntity roulette)
        {
            if (bets.Count == 0) return;

            foreach (var bet in bets)
            {
                bet.GetResult(roulette);

                if (bet.Result == BetResult.Win)
                {
                    var gambler = await _gamblerRepository.FindSingleOrDefaultAsync(g => g.Id == bet.GamblerId);
                    if (gambler is not null)
                    {
                        gambler.PayCredit(bet.Winnings);
                        await _gamblerRepository.EditAsync(gambler);
                    }
                }

                await _betRepository.EditAsync(bet);
            }
        }

        private static BetDto MapBetToResponse(BetEntity bet) =>
            new(
                bet.Id,
                bet.Amount,
                bet.BetType.ToString(),
                bet.BetType == BetType.Number ? bet.Number?.ToString() ?? "N/A" : bet.Color?.ToString() ?? "N/A",
                bet.Result.ToString(),
                bet.Winnings,
                bet.CreatedAt
            );
    }
}
