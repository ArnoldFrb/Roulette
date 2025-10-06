using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;

namespace Roulette.Application.UserServices
{
    public class AuthenticationService(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public UserResponse Authenticate(string userName, string password)
        {
            try
            {
                var user = _userRepository.FindSingleOrDefault(u => u.Username == userName);
                if (user == null)
                    return ErrorResponse("User not found.");

                if (!user.IsPassword(password))
                    return ErrorResponse("Invalid password.");

                return new UserResponse(user.Id, user.Username, "Authentication successful.");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred during authentication.\nException: {ex.Message}");
            }
        }

        private static UserResponse ErrorResponse(string message) =>
            new(null, null, message);
    }
}
