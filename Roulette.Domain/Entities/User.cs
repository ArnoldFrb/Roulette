using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public class User(String username, String password, decimal credit) : Entity<int>
    {
        public String Username { get; protected set; } = username;
        public String Password { get; protected set; } = password;
        public decimal Credit { get; protected set; } = credit;

        public void IsValidPassword(string password)
        {
            if (Password != password || string.IsNullOrEmpty(password))
                throw new GenericException("Invalid password.");
        }

        public void IsValidUsername(string username)
        {
            if (Username != username || string.IsNullOrEmpty(username))
                throw new GenericException("Invalid username.");
        }
        
        public void AddCredit(decimal amount)
        {
            if (amount <= 0)
                throw new GenericException("Invalid credits amount. Must be greater than 0.");
            Credit += amount;
        }

        public void DeductCredit(decimal amount)
        {
            if (amount <= 0)
                throw new GenericException("Invalid credits amount. Must be greater than 0.");
            if (Credit < amount)
                throw new GenericException("Insufficient credits.");
            Credit -= amount;
        }
    }
}
