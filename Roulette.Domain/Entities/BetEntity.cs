using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public enum BetType
    {
        Number,
        Color
    }

    public class BetEntity : Entity<int>
    {

        public decimal Amount { get; protected set; }
        public BetType BetType { get; protected set; }
        public RouletteColor? Color { get; protected set; }
        public int? Number { get; protected set; }

        public int UserId { get; protected set; }
        public UserEntity User { get; protected set; } = default!;

        public int RouletteId { get; protected set; }
        public RouletteEntity Roulette { get; protected set; } = default!;

        public BetEntity(decimal amount, BetType betType, RouletteColor? color, int? number, int userId, int rouletteId)
        {
            Amount = ValidateAmount(amount);
            BetType = betType;
            Color = color;
            Number = number;
            UserId = userId;
            RouletteId = rouletteId;
        }

        public BetEntity() { }

        public void ValidateBet()
        {
            if (BetType == BetType.Color)
            {
                if (Color is null || (Color != RouletteColor.Red && Color != RouletteColor.Black))
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

        public static decimal ValidateAmount(decimal amount)
        {
            if (!IsValidAmount(amount))
                throw new InvalidBetAmountException();
            return amount;
        }

        public static bool IsValidAmount(decimal amount) => amount > RouletteConstants.MinBet && amount <= RouletteConstants.MaxBet;

        public static bool IsValidNumber(int number) => number >= RouletteConstants.MinNumber && number <= RouletteConstants.MaxNumber;

        public static bool IsValidColor(string color) =>
            color.Equals(nameof(RouletteColor.Red), StringComparison.CurrentCultureIgnoreCase) ||
            color.Equals(nameof(RouletteColor.Black), StringComparison.CurrentCultureIgnoreCase);
    }
}
