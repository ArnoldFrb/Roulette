using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public class GamblerEntity : UserEntity
    {
        public decimal Credit { get; protected set; } = decimal.Zero;
        public ICollection<BetEntity> Bets { get; protected set; } = [];
        protected GamblerEntity() { }

        public GamblerEntity(string username, decimal credit) : base(username)
        {
            Credit = credit;
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
                throw new InvalidCreditOperationException();
            Credit -= amount;
        }

        private static void ValidateCredit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidCreditOperationException("Invalid credits amount. Must be greater than 0.");
        }

        private void EnsureHasSufficientCredit(decimal amount)
        {
            if (Credit < amount)
                throw new InvalidCreditOperationException("Not enough credit to perform this operation.");
        }
    }
}
