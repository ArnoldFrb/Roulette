using Roulette.Application.Models;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.User;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services.User;
using Roulette.Domain.Entities;

namespace Roulette.Application.UserServices
{
    public class AuthenticationService(IUserRepository userRepository) : IAuthenticationService<AuthenticationRequest, AuthenticationResponse>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<AuthenticationResponse> ExecuteAsync(AuthenticationRequest request)
        {
            try
            {
                UserEntity. IsValidUsername(request.UserName);
                UserEntity.IsValidPassword(request.Password);

                var user = await _userRepository.FindSingleOrDefaultAsync(u => u.Username == request.UserName);
                if (user == null)
                    return AuthenticationResponse.Fail(AppCodes.User.USER_NOT_FOUND, "User not found.");

                if (!user.ValidatePassword(request.Password))
                    return AuthenticationResponse.Fail(AppCodes.Auth.AUTH_FAILED, "Invalid password.");

                return AuthenticationResponse.Success(new AuthenticationDto(user.Id, user.Username));
            }
            catch (Exception ex)
            {
                return AuthenticationResponse.Fail(AppCodes.System.INTERNAL_ERROR, ex.Message);
            }
        }
    }
}
