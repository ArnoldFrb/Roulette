using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public class UserEntity(string username, string password, decimal credit) : Entity<int>
    {
        public string Username { get; protected set; } = username;
        public string Password { get; protected set; } = password;
        public decimal Credit { get; protected set; } = credit;

        public bool ValidatePassword(string password)
        {
            return Password.Equals(password);
        }

        public void IsValidPassword(string password)
        {
            if (Password != password || string.IsNullOrWhiteSpace(password))
                throw new InvalidPasswordException();
        }

        public void IsValidUsername(string username)
        {
            if (Username != username || string.IsNullOrWhiteSpace(username))
                throw new InvalidUsernameException();
        }

        public void IncreaseCredit(decimal amount)
        {
            IsValidAmount(amount);
            Credit += amount;
        }

        public void DeductCredit(decimal amount)
        {
            IsValidAmount(amount);
            if (Credit < amount)
                throw new InsufficientCreditsException();
            Credit -= amount;
        }

        private static void IsValidAmount(decimal amount)
        {
            if (amount <= 0)
                throw new InsufficientCreditsException("Invalid credits amount. Must be greater than 0.");
        }
    }
}
