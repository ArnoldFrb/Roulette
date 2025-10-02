using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public class User(String username, String password) : Entity<int>
    {
        public String Username { get; protected set; } = username;
        public String Password { get; protected set; } = password;

        public void IsValidPassword(String password)
        {
            if (Password != password)
                throw new GenericException("Invalid password.");
        }

        public void IsValidUsername(String username)
        {
            if (Username != username)
                throw new GenericException("Invalid username.");
        }
    }
}
