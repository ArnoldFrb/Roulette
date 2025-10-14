using Microsoft.Extensions.Logging;
using Roulette.Application.Models;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Contracts.Services.Roulette;
using Roulette.Domain.Entities;

namespace Roulette.Application.RouletteServices
{
    public class CreateRouletteService(IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork, IRedisCacheService redis, ILogger<CreateRouletteService> logger) : ICreateRouletteService<CreateRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRedisCacheService _redis = redis;
        private readonly ILogger<CreateRouletteService> _logger = logger;

        public async Task<CreateRouletteResponse> ExecuteAsync()
        {
            _logger.LogInformation("Attempt to create a roulette");
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var roulette = new RouletteEntity();

                await _rouletteRepository.AddAsync(roulette);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _redis.RemoveAsync(GetAllRouletteService.CacheKey);

                return CreateRouletteResponse.Success(new CreateRouletteDto()
                {
                    Id = roulette.Id,
                    Status = roulette.Status.ToString(),
                    CreatedAt = roulette.CreatedAt,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Attempt to create a roulette");
                await _unitOfWork.RollbackTransactionAsync();
                return CreateRouletteResponse.Fail(AppCodes.System.INTERNAL_ERROR, ex.Message);
            }
        }
    }
}
