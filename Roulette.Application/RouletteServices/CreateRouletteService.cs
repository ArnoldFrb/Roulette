using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Entities;

namespace Roulette.Application.RouletteServices
{
    public class CreateRouletteService(IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork)
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public CreateRouletteResponse Execute()
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var roulette = new RouletteEntity();
                _rouletteRepository.Add(roulette);
                _unitOfWork.CommitTransaction();

                return CreateRouletteResponse.Success(roulette.Id, roulette.Status.ToString(), roulette.CreatedAt);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return CreateRouletteResponse.Fail($"Error creating roulette: {ex.Message}");
            }
        }
    }
}
