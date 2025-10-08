using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Contracts.Services.Bet;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Application.BetServices
{
    public class CreateBetService(IBetRepository betRepository, IUserRepository userRepository, IRouletteRepository rouletteRepository, IUnitOfWork unitOfWork) : ICreateBetService<CreateBetRequest, CreateBetResponse>
    {
        private readonly IBetRepository _betRepository = betRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRouletteRepository _rouletteRepository = rouletteRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<CreateBetResponse> ExecuteAsync(int userId, CreateBetRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.FindSingleOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return CreateBetResponse.Fail("User not found.");

                var roulette = await _rouletteRepository.FindSingleOrDefaultAsync(r => r.Id == request.RouletteId);
                if (roulette == null)
                    return CreateBetResponse.Fail("Roulette not found.");

                roulette.EnsureIsOpenForBets();

                var existingBet = await _betRepository.FindSingleOrDefaultAsync(b => b.Roulette.Id == roulette.Id && b.User.Id == user.Id);
                if (existingBet != null)
                    return CreateBetResponse.Fail("You already placed a bet on this roulette.");

                var bet = CreateBet(request, user, roulette);
                bet.ValidateBet();

                user.DeductCredit(request.Amount);
                await _userRepository.EditAsync(user);

                await _betRepository.AddAsync(bet);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return CreateBetResponse.Success(
                    bet.Amount,
                    bet.BetType.ToString(),
                    bet.BetType == BetType.Number ? bet.Number?.ToString() ?? "N/A" : bet.Color?.ToString() ?? "N/A"
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return CreateBetResponse.Fail(ex.Message);
            }
        }

        private static BetEntity CreateBet(CreateBetRequest request, UserEntity user, RouletteEntity roulette)
        {
            return request.Type switch
            {
                BetType.Number => CreateNumberBet(request, user, roulette),
                BetType.Color => CreateColorBet(request, user, roulette),
                _ => throw new InvalidBetTypeException()
            };
        }

        private static BetEntity CreateNumberBet(CreateBetRequest request, UserEntity user, RouletteEntity roulette)
        {
            if (!int.TryParse(request.Value, out int number) || !BetEntity.IsValidNumber(number))
                    throw new InvalidBetNumberException();

            return new BetEntity(request.Amount, BetType.Number, null, number, user, roulette);
        }

        private static BetEntity CreateColorBet(CreateBetRequest request, UserEntity user, RouletteEntity roulette)
        {
            if (!Enum.TryParse<RouletteColor>(request.Value, true, out var color) || !BetEntity.IsValidColor(request.Value))
                throw new InvalidBetColorException();

            return new BetEntity(request.Amount, BetType.Color, color, null, user, roulette);
        }
    }
}
