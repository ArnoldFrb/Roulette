using Roulette.Application.Models;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.Bet;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Contracts.Services.Bet;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Application.BetServices
{
    public class CreateBetService(IBetRepository betRepository, IGamblerRepository gamblerRepository, IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork) : ICreateBetService<CreateBetRequest, CreateBetResponse>
    {
        private readonly IBetRepository _betRepository = betRepository;
        private readonly IGamblerRepository _gamblerRepository = gamblerRepository;
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<CreateBetResponse> ExecuteAsync(int userId, CreateBetRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _gamblerRepository.FindSingleOrDefaultAsync(u => u.Id == userId);
                if (user is null)
                    return CreateBetResponse.Fail(AppCodes.User.USER_NOT_FOUND, "User not found.");

                var roulette = await _rouletteRepository.FindSingleOrDefaultAsync(r => r.Id == request.RouletteId);
                if (roulette is null)
                    return CreateBetResponse.Fail(AppCodes.Roulette.ROULETTE_NOT_FOUND, "Roulette not found.");

                roulette.EnsureIsOpenForBets();

                var existingBet = await _betRepository.FindSingleOrDefaultAsync(b => b.RouletteId == roulette.Id && b.GamblerId == user.Id);
                if (existingBet is not null)
                    return CreateBetResponse.Fail(AppCodes.Bet.BET_CREATION_ERROR, "You already placed a bet on this roulette.");

                var bet = CreateBet(request, user, roulette);
                bet.ValidateBet();

                user.DeductCredit(request.Amount);
                await _gamblerRepository.EditAsync(user);
                await _betRepository.AddAsync(bet);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return CreateBetResponse.Success(
                    new CreateBetDto(
                        bet.Amount,
                        bet.BetType.ToString(),
                        bet.BetType == BetType.Number ? bet.Number?.ToString() ?? "N/A" : bet.Color?.ToString() ?? "N/A"
                    )
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                var code = ex switch
                {
                    InvalidOperationException => AppCodes.Bet.BET_CREATION_ERROR,
                    InvalidBetNumberException => AppCodes.Bet.INVALID_BET_NUMBER,
                    InvalidBetColorException => AppCodes.Bet.INVALID_BET_COLOR,
                    InvalidCreditOperationException => AppCodes.Bet.INVALID_BET_AMOUNT,
                    _ => AppCodes.System.INTERNAL_ERROR
                };

                return CreateBetResponse.Fail(code, ex.Message);
            }
        }

        private static BetEntity CreateBet(CreateBetRequest request, GamblerEntity user, RouletteEntity roulette)
        {
            return request.Type switch
            {
                BetType.Number => CreateNumberBet(request, user.Id, roulette.Id),
                BetType.Color => CreateColorBet(request, user.Id, roulette.Id),
                _ => throw new InvalidBetTypeException()
            };
        }

        private static BetEntity CreateNumberBet(CreateBetRequest request, int userId, int rouletteId)
        {
            if (!int.TryParse(request.Value, out int number))
                throw new InvalidBetNumberException("The provided number is not valid.");

            if (number < RouletteConstants.MinBet || number > RouletteConstants.MaxBet)
                throw new InvalidBetNumberException();

            return new BetEntity(request.Amount, BetType.Number, null, number, userId, rouletteId);
        }

        private static BetEntity CreateColorBet(CreateBetRequest request, int userId, int rouletteId)
        {
            if (!Enum.TryParse<RouletteColor>(request.Value, true, out var color))
                throw new InvalidBetColorException($"Color '{request.Value}' is not valid. Must be RED or BLACK.");

            if (color != RouletteColor.Red && color != RouletteColor.Black)
                throw new InvalidBetColorException();

            return new BetEntity(request.Amount, BetType.Color, color, null, userId, rouletteId);
        }
    }
}
