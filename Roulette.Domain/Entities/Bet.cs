using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public enum BetType
    {
        Number,
        Color
    }

    public class Bet(decimal amount, BetType betType, string? color, int? number, User user, Roulette roulette) : Entity<int>
    {
        public decimal Amount { get; protected set; } = amount;
        public BetType BetType { get; protected set; } = betType;
        public string? Color { get; protected set; } = color;
        public int? Number { get; protected set; } = number;
        public User User { get; protected set; } = user;
        public Roulette Roulette { get; protected set; } = roulette;

        public void IsValidBet()
        {
            if (Amount <= 0 && Amount > 10000)
                throw new GenericException("Bet amount must be between 0 and 100000.");
            if (BetType == BetType.Color)
            {
                if (Color is null || (!Color.Equals("red", StringComparison.CurrentCultureIgnoreCase) && !Color.Equals("black", StringComparison.CurrentCultureIgnoreCase)))
                    throw new GenericException("Invalid color bet. Must be 'red' or 'black'.");
            }
            else if (BetType == BetType.Number)
            {
                if (Number is null || Number < 0 || Number > 36)
                    throw new GenericException("Invalid number bet. Must be between 0 and 36.");
            }
            else
            {
                throw new GenericException("Invalid bet type.");
            }
        }

        public bool IsWinner()
        {
            if (BetType == BetType.Color)
            {
                return Color?.Equals(Roulette.ColorWinner.ToString(), StringComparison.CurrentCultureIgnoreCase) ?? false;
            }
            else if (BetType == BetType.Number)
            {
                return Number == Roulette.NumberWinner;
            }
            return false;
        }

        public decimal GetWinnings()
        {
            if (IsWinner())
            {
                if (BetType == BetType.Color)
                {
                    return Amount * 1.8m;
                }
                else if (BetType == BetType.Number)
                {
                    return Amount * 5m;
                }
            }
            return 0;
        }
    }
}
