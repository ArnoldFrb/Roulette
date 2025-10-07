using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Entities;

namespace Roulette.Application.UserServices
{
    public class AuthenticationService(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public AuthenticationResponse Authenticate(AuthenticationRequest request)
        {
            try
            {
                UserEntity. IsValidUsername(request.UserName);
                UserEntity.IsValidPassword(request.Password);

                var user = _userRepository.FindSingleOrDefault(u => u.Username == request.UserName);
                if (user == null)
                    return AuthenticationResponse.Fail("User not found.");

                if (!user.ValidatePassword(request.Password))
                    return AuthenticationResponse.Fail("Invalid password.");

                return AuthenticationResponse.Success(user.Id, user.Username);
            }
            catch (Exception ex)
            {
                return AuthenticationResponse.Fail(ex.Message);
            }
        }
    }
}
