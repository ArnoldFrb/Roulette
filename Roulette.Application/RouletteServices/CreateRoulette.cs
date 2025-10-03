using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;

namespace Roulette.Application.RouletteServices
{
    public class CreateRoulette(IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork)
    {
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public RouletteResponse Execute()
        {
            try
            {
                var roulette = new Domain.Entities.Roulette();
                _rouletteRepository.Add(roulette);
                _unitOfWork.Commit();

                return new RouletteResponse(roulette.Id, roulette.Status.ToString(), roulette.CreatedAt, "Roulette created successfully.");
            }
            catch (Exception ex)
            {
                return new RouletteResponse(null, null, null, $"Error creating roulette: {ex.Message}");

            }
        }
    }
}
