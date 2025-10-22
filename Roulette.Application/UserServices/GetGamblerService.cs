using Microsoft.Extensions.Logging;
using Roulette.Application.Models;
using Roulette.Application.Models.Responses.User;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services.User;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Application.UserServices
{
    public class GetGamblerService(IGamblerRepository gamblerRepository, ILogger<GetGamblerService> logger) : IGetGamblerService<GamblerResponse>
    {
        private readonly IGamblerRepository _gamblerRepository = gamblerRepository;
        private readonly ILogger<GetGamblerService> _logger = logger;
        public async Task<GamblerResponse> ExecuteAsync(string username)
        {
            _logger.LogInformation("Attempt to obtain the user {Username}", username);
            try
            {
                var gambler = await _gamblerRepository.FindSingleOrDefaultAsync(g => g.Username == username);
                if (gambler == null)
                    return GamblerResponse.Fail(AppCodes.User.USER_NOT_FOUND, "Gambler not found.");
                return GamblerResponse.Success(new GamblerDto(gambler.Id, gambler.Username, gambler.Credit));
            }
            catch (InvalidUsernameOrPasswordException ex)
            {
                _logger.LogError(ex, "Error searching for user {Username}", username);
                return GamblerResponse.Fail(AppCodes.User.INVALID_CREDENTIALS, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for user {Username}", username);
                return GamblerResponse.Fail(AppCodes.System.INTERNAL_ERROR, ex.Message);
            }
        }
    }
}
