using Microsoft.Extensions.Logging;
using Roulette.Application.Models;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.User;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services.User;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Application.UserServices
{
    public class AuthenticationService(ICrupierRepository userRepository, ILogger<AuthenticationService> logger) : IAuthenticationService<AuthenticationRequest, CrupierResponse>
    {
        private readonly ICrupierRepository _userRepository = userRepository;
        private readonly ILogger<AuthenticationService> _logger = logger;

        public async Task<CrupierResponse> ExecuteAsync(AuthenticationRequest request)
        {
            _logger.LogInformation("Authentication attempt for user {UserName}", request.Username);
            try
            {
                UserEntity.IsValidUsername(request.Username);
                CrupierEntity.IsValidPassword(request.Password);

                var user = await _userRepository.FindSingleOrDefaultAsync(u => u.Username == request.Username);
                if (user == null)
                    return CrupierResponse.Fail(AppCodes.User.USER_NOT_FOUND, "User not found.");

                if (!user.ValidatePassword(request.Password))
                    return CrupierResponse.Fail(AppCodes.Auth.AUTH_FAILED, "Invalid password.");

                return CrupierResponse.Success(new CrupierDto(user.Id, user.Username, string.Empty));
            }
            catch (InvalidUsernameOrPasswordException ex)
            {
                _logger.LogError(ex, "Error authenticating user {UserName}", request.Username);
                return CrupierResponse.Fail(AppCodes.Auth.INVALID_AUTH, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user {UserName}", request.Username);
                return CrupierResponse.Fail(AppCodes.System.INTERNAL_ERROR, ex.Message);
            }
        }
    }
}
