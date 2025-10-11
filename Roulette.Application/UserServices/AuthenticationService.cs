using Roulette.Application.Models;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.User;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services.User;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Application.UserServices
{
    public class AuthenticationService(ICrupierRepository userRepository) : IAuthenticationService<AuthenticationRequest, UserResponse>
    {
        private readonly ICrupierRepository _userRepository = userRepository;

        public async Task<UserResponse> ExecuteAsync(AuthenticationRequest request)
        {
            try
            {
                UserEntity.IsValidUsername(request.UserName);
                CrupierEntity.IsValidPassword(request.Password);

                var user = await _userRepository.FindSingleOrDefaultAsync(u => u.Username == request.UserName);
                if (user == null)
                    return UserResponse.Fail(AppCodes.User.USER_NOT_FOUND, "User not found.");

                if (!user.ValidatePassword(request.Password))
                    return UserResponse.Fail(AppCodes.Auth.AUTH_FAILED, "Invalid password.");

                return UserResponse.Success(new UserDto(user.Id, user.Username));
            }
            catch (InvalidUsernameOrPasswordException ex)
            {
                return UserResponse.Fail(AppCodes.Auth.INVALID_AUTH, ex.Message);
            }
            catch (Exception ex)
            {
                return UserResponse.Fail(AppCodes.System.INTERNAL_ERROR, ex.Message);
            }
        }
    }
}
