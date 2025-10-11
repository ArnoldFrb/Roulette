using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public enum BetType
    {
        Number,
        Color
    }

    public enum BetResult{
        Win,
        Lose,
        Pending
    }

    public class BetEntity : Entity<int>
    {

        public decimal Amount { get; protected set; }
        public BetType BetType { get; protected set; }
        public RouletteColor? Color { get; protected set; }
        public int? Number { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public decimal Winnings { get; protected set; }
        public BetResult Result { get; set; }

        public int GamblerId { get; protected set; }
        public GamblerEntity Gambler { get; protected set; } = default!;
        public int RouletteId { get; protected set; }
        public RouletteEntity Roulette { get; protected set; } = default!;

        private BetEntity() { }

        public BetEntity(decimal amount, BetType betType, RouletteColor? color, int? number, int gamblerId, int rouletteId)
        {
            Amount = ValidateAmount(amount);
            BetType = betType;
            Color = color;
            Number = number;
            GamblerId = gamblerId;
            RouletteId = rouletteId;
            CreatedAt = DateTime.UtcNow;
            Winnings = decimal.Zero;
            Result = BetResult.Pending;

            ValidateBet();
        }

        public void ValidateBet()
        {
            if (BetType == BetType.Color) EnsureValidColor();
            if (BetType == BetType.Number) EnsureValidNumber();
        }

        private void EnsureValidColor()
        {
            if (Color is null || (Color != RouletteColor.Red && Color != RouletteColor.Black))
                throw new InvalidBetColorException();
        }

        private void EnsureValidNumber()
        {
            if (Number is null || Number < RouletteConstants.MinNumber || Number > RouletteConstants.MaxNumber)
                throw new InvalidBetNumberException();
        }

        public void GetResult(RouletteEntity roulette)
        {
            Result = IsWinner(roulette) ? BetResult.Win : BetResult.Lose;
            if (Result == BetResult.Win)
                CalculateWinnings();
        }

        private bool IsWinner(RouletteEntity roulette) =>
            BetType switch
            {
                BetType.Color => Color == roulette.ColorWinner,
                BetType.Number => Number == roulette.NumberWinner,
                _ => false
            };

        private void CalculateWinnings()
        {
            Winnings = BetType switch
            {
                BetType.Color => Amount * 1.8m,
                BetType.Number => Amount * 5m,
                _ => 0m
            };
        }

        public static decimal ValidateAmount(decimal amount)
        {
            if (!IsValidAmount(amount))
                throw new InvalidBetAmountException();
            return amount;
        }

        public static bool IsValidAmount(decimal amount) =>
            amount > RouletteConstants.MinBet && amount <= RouletteConstants.MaxBet;
    }
}
