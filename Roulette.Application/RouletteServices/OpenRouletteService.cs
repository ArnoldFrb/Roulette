using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Contracts.Services.Roulette;

namespace Roulette.Application.RouletteServices
{
    public class OpenRouletteService(IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork, IRedisCacheService redis) : IOpenRouletteService<OpenRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRedisCacheService _redis = redis;

        public async Task<OpenRouletteResponse> ExecuteAsync(int rouletteId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var roulette = await _rouletteRepository.FindSingleOrDefaultAsync(r => r.Id == rouletteId);
                if (roulette == null)
                    return OpenRouletteResponse.Fail("Roulette not found.");

                roulette.OpenBet();
                await _rouletteRepository.EditAsync(roulette);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _redis.RemoveAsync(GetAllRouletteService.CacheKey);

                return OpenRouletteResponse.Success(roulette.Id, roulette.Status.ToString(), roulette.CreatedAt, roulette.OpenedAt);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return OpenRouletteResponse.Fail(ex.Message);
            }
        }
    }
}
