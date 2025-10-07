using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public class UserEntity(string username, string password, decimal credit, bool isAdmin = false) : Entity<int>
    {
        public string Username { get; protected set; } = username;
        public string Password { get; protected set; } = password;
        public decimal Credit { get; protected set; } = credit;
        public bool IsAdmin { get; protected set; } = isAdmin;

        public bool ValidatePassword(string password)
        {
            return Password.Equals(password);
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidUsernameOrPasswordException();
            return true;
        }

        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidUsernameOrPasswordException();
            return true;
        }

        public void PayCredit(decimal amount)
        {
            ValidateCredit(amount);
            Credit += amount;
        }

        public void DeductCredit(decimal amount)
        {
            ValidateCredit(amount);
            EnsureHasSufficientCredit(amount);
            if (Credit < amount)
                throw new InsufficientCreditsException();
            Credit -= amount;
        }

        private static void ValidateCredit(decimal amount)
        {
            if (amount <= 0)
                throw new InsufficientCreditsException("Invalid credits amount. Must be greater than 0.");
        }

        public void EnsureHasSufficientCredit(decimal amount)
        {
            if (Credit < amount)
                throw new InsufficientCreditsException();
        }
    }
}
