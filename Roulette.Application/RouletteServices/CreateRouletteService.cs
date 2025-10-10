using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Contracts.Services.Roulette;
using Roulette.Domain.Entities;

namespace Roulette.Application.RouletteServices
{
    public class CreateRouletteService(IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork, IRedisCacheService redis) : ICreateRouletteService<CreateRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRedisCacheService _redis = redis;

        public async Task<CreateRouletteResponse> ExecuteAsync()
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var roulette = new RouletteEntity();
                await _rouletteRepository.AddAsync(roulette);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _redis.RemoveAsync(GetAllRouletteService.CacheKey);

                return CreateRouletteResponse.Success(roulette.Id, roulette.Status.ToString(), roulette.CreatedAt);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return CreateRouletteResponse.Fail(ex.Message);
            }
        }
    }
}
