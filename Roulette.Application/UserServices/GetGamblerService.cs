using Roulette.Application.Models;
using Roulette.Application.Models.Responses.User;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services.User;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Application.UserServices
{
    public class GetGamblerService(IGamblerRepository gamblerRepository) : IGetGamblerService<UserResponse>
    {
        private readonly IGamblerRepository _gamblerRepository = gamblerRepository;
        public async Task<UserResponse> ExecuteAsync(string username)
        {
            try
            {
                UserEntity.IsValidUsername(username);

                var gambler = await _gamblerRepository.FindSingleOrDefaultAsync(g => g.Username == username);
                if (gambler == null)
                    return UserResponse.Fail(AppCodes.User.USER_NOT_FOUND, "Gambler not found.");
                return UserResponse.Success(new UserDto(gambler.Id, gambler.Username));
            }
            catch (InvalidUsernameOrPasswordException ex)
            {
                return UserResponse.Fail(AppCodes.User.INVALID_CREDENTIALS, ex.Message);
            }
            catch (Exception ex)
            {
                return UserResponse.Fail(AppCodes.System.INTERNAL_ERROR, ex.Message);
            }
        }
    }
}
