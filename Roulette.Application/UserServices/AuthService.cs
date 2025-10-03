using Roulette.Application.Models.Responses;
using Roulette.Domain.Contracts.Repositories;

namespace Roulette.Application.UserServices
{
    public class AuthService(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public UserResponse Authenticate(string userName, string password)
        {
            try
            {
                var user = _userRepository.FindSingleOrDefault(u => u.Username == userName);
                if (user == null)
                    return new UserResponse(null, null, "User not found.");

                if (!user.IsPassword(password))
                    return new UserResponse(null, null, "Invalid password.");

                return new UserResponse(user.Id, user.Username, "Authentication successful.");
            }
            catch (Exception ex)
            {
                return new UserResponse(null, null, $"An error occurred during authentication.\nException: {ex.Message}");
            }
        }
    }
}
