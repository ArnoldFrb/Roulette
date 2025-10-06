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
            try
            {
                var roulette = new RouletteEntity();
                _rouletteRepository.Add(roulette);
                _unitOfWork.Commit();

                return new CreateRouletteResponse(roulette.Id, roulette.Status.ToString(), roulette.CreatedAt, "Roulette created successfully.");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Error creating roulette: {ex.Message}");
            }
        }

        private static CreateRouletteResponse ErrorResponse(string message) =>
            new(null, null, null, message);
    }
}
