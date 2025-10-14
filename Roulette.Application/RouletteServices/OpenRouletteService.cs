using Microsoft.Extensions.Logging;
using Roulette.Application.Models;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Contracts.Services.Roulette;
using Roulette.Domain.Entities.Exceptions;
using static Roulette.Application.Models.AppCodes;

namespace Roulette.Application.RouletteServices
{
    public class OpenRouletteService(IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork, IRedisCacheService redis, ILogger<OpenRouletteService> logger) : IOpenRouletteService<OpenRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRedisCacheService _redis = redis;
        private readonly ILogger<OpenRouletteService> _logger = logger;

        public async Task<OpenRouletteResponse> ExecuteAsync(int rouletteId)
        {
            _logger.LogInformation("Attempt to open a roulette {RouletteId}", rouletteId);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var roulette = await _rouletteRepository.FindSingleOrDefaultAsync(r => r.Id == rouletteId);
                if (roulette is null)
                    return OpenRouletteResponse.Fail(AppCodes.Roulette.ROULETTE_NOT_FOUND, "Roulette not found.");

                roulette.OpenRoulette();
                await _rouletteRepository.EditAsync(roulette);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _redis.RemoveAsync(GetAllRouletteService.CacheKey);

                return OpenRouletteResponse.Success(new OpenRouletteDto()
                {
                    Id = roulette.Id,
                    Status = roulette.Status.ToString(),
                    CreatedAt = roulette.CreatedAt,
                    OpenedAt = roulette.OpenedAt
                });
            }
            catch (InvalidRouletteStatusException ex)
            {
                _logger.LogError(ex, "Attempt to open a roulette {RouletteId}", rouletteId);

                await _unitOfWork.RollbackTransactionAsync();
                return OpenRouletteResponse.Fail(AppCodes.Roulette.ROULETTE_OPEN_ERROR, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Attempt to open a roulette {RouletteId}", rouletteId);

                await _unitOfWork.RollbackTransactionAsync();
                return OpenRouletteResponse.Fail(AppCodes.System.INTERNAL_ERROR, ex.Message);
            }
        }
    }
}
