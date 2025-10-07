using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Application.BetServices
{
    public class CreateBetService(IBetRepository betRepository, IUnitOfWork unitOfWork)
    {
        private readonly IBetRepository _betRepository = betRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public CreateBetResponse Execute(CreateBetRequest request, UserEntity user, RouletteEntity roulette)
        {
            try
            {
                if (request.Type == BetType.Number)
                    return CreateAndSaveBet(() => CreateNumberBet(request, user, roulette));
                else if (request.Type == BetType.Color)
                    return CreateAndSaveBet(() => CreateColorBet(request, user, roulette));
                else
                    return ErrorResponse("Invalid bet type.");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Error creating bet: {ex.Message}");
            }
        }
        private CreateBetResponse CreateAndSaveBet(Func<BetEntity> createBetFunc)
        {
            var bet = createBetFunc();
            bet.IsValidBet();
            _betRepository.Add(bet);
            _unitOfWork.Commit();
            return new CreateBetResponse(
                bet.Amount,
                bet.BetType.ToString(),
                bet.BetType == BetType.Number ? bet.Number?.ToString() ?? "N/A" : bet.Color?.ToString() ?? "N/A",
                "Bet created successfully."
                );
        }

        private static BetEntity CreateNumberBet(CreateBetRequest request, UserEntity user, RouletteEntity roulette)
        {
            if (!int.TryParse(request.Value, out int number))
                throw new InvalidBetNumberException();

            if (BetEntity.IsValidNumber(number))
            {
                return new BetEntity(request.Amount, BetType.Number, null, number, user, roulette);
            }

            throw new InvalidBetNumberException();
        }

        private static BetEntity CreateColorBet(CreateBetRequest request, UserEntity user, RouletteEntity roulette)
        {
            if (!Enum.TryParse<BetColor>(request.Value, true, out var color))
                throw new InvalidBetColorException();

            if (BetEntity.IsValidColor(request.Value))
            {
                return new BetEntity(request.Amount, BetType.Color, color, null, user, roulette);
            }

            throw new InvalidBetColorException();
        }

        private static CreateBetResponse ErrorResponse(string message) =>
            new(null, null, null, message);
    }
}
