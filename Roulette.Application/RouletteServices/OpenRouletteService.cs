using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Contracts.Services.Roulette;

namespace Roulette.Application.RouletteServices
{
    public class OpenRouletteService(IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork) : IOpenRouletteService<OpenRouletteResponse>
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public OpenRouletteResponse Execute(int rouletteId)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var roulette = _rouletteRepository.FindSingleOrDefault(r => r.Id == rouletteId);
                if (roulette == null)
                    return OpenRouletteResponse.Fail("Roulette not found.");

                roulette.OpenBet();
                _rouletteRepository.Edit(roulette);
                _unitOfWork.CommitTransaction();

                return OpenRouletteResponse.Success(roulette.Id, roulette.Status.ToString(), roulette.CreatedAt, roulette.OpenedAt);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
                return OpenRouletteResponse.Fail(ex.Message);
            }
        }
    }
}
