using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public class UserEntity : Entity<int>
    {

        public string Username { get; protected set; } = string.Empty;

        protected UserEntity() { }
        public UserEntity(string username)
        {
            Username = username;
        }

        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidUsernameOrPasswordException();
            return true;
        }
    }
}
