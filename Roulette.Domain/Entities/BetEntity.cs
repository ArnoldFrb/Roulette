using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public enum BetType
    {
        Number,
        Color
    }

    public class BetEntity(decimal amount, BetType betType, BetColor? color, int? number, UserEntity user, RouletteEntity roulette) : Entity<int>
    {
        public decimal Amount { get; protected set; } = IsValidAmount(amount);
        public BetType BetType { get; protected set; } = betType;
        public BetColor? Color { get; protected set; } = color;
        public int? Number { get; protected set; } = number;
        public UserEntity User { get; protected set; } = user;
        public RouletteEntity Roulette { get; protected set; } = roulette;

        public void IsValidBet()
        {
            if (BetType == BetType.Color)
            {
                if (Color is null || (Color == BetColor.Red && Color == BetColor.Black))
                    throw new InvalidBetColorException();
            }
            else if (BetType == BetType.Number)
            {
                if (Number is null || Number < RouletteConstants.MinNumber || Number > RouletteConstants.MaxNumber)
                    throw new InvalidBetNumberException();
            }
            else
            {
                throw new InvalidBetTypeException();
            }
        }

        public bool IsWinner()
        {
            if (BetType == BetType.Color)
            {
                return Color == Roulette.ColorWinner;
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

        public static decimal IsValidAmount(decimal amount)
        {
            if (amount <= RouletteConstants.MinBet || amount > RouletteConstants.MaxBet)
                throw new InvalidBetAmountException();
            return amount;
        }

        public static bool IsValidNumber(int number) => number >= RouletteConstants.MinNumber && number <= RouletteConstants.MaxNumber;

        public static bool IsValidColor(string color) =>
            color.Equals(nameof(BetColor.Red), StringComparison.CurrentCultureIgnoreCase) ||
            color.Equals(nameof(BetColor.Black), StringComparison.CurrentCultureIgnoreCase);
    }

    public static class RouletteConstants
    {
        public const int MinNumber = 0;
        public const int MaxNumber = 36;
        public const decimal MinBet = 0;
        public const decimal MaxBet = 10000;
    }
}
