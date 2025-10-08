using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public enum RouletteStatus
    {
        Created,
        Open,
        Closed
    }

    public enum RouletteColor
    {
        Red,
        Black,
        Colorless
    }

    public class RouletteEntity : Entity<int>
    {
        private static readonly Random _random = new();

        public RouletteEntity()
        {
            RouletteId = 0;
            NumberWinner = -1;
            ColorWinner = RouletteColor.Colorless;
            Status = RouletteStatus.Created;
            CreatedAt = DateTime.UtcNow;
            OpenedAt = DateTime.MinValue;
            ClosedAt = DateTime.MinValue;
        }

        public int RouletteId { get; protected set; }
        public int NumberWinner { get; protected set; }
        public RouletteColor ColorWinner { get; protected set; }
        public RouletteStatus Status { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime OpenedAt { get; protected set; }
        public DateTime ClosedAt { get; protected set; }

        public void OpenBet()
        {
            EnsureHasStatus(RouletteStatus.Created);
            Status = RouletteStatus.Open;
            OpenedAt = DateTime.UtcNow;
        }

        public void CloseBet()
        {
            EnsureHasStatus(RouletteStatus.Open);
            Status = RouletteStatus.Closed;
            ClosedAt = DateTime.UtcNow;
        }

        private void EnsureHasStatus(RouletteStatus expected)
        {
            if (Status == RouletteStatus.Closed)
                throw new InvalidRouletteStatusException("The roulette is already closed.");

            if (Status != expected)
                throw new InvalidRouletteStatusException($"Expected status {expected}, but current is {Status}.");
        }
        public void GenerateWinningBet()
        {
            NumberWinner = IsValidNumberWinner(_random.Next(RouletteConstants.MinNumber, RouletteConstants.MaxNumber+1));
            ColorWinner = GetWinnerColor(NumberWinner);
        }

        public static RouletteColor GetWinnerColor(int numberWinner) => (numberWinner % 2 == 0) ? RouletteColor.Red : RouletteColor.Black;

        public static int IsValidNumberWinner(int numberWinner) {
            if (numberWinner < RouletteConstants.MinNumber || numberWinner > RouletteConstants.MaxNumber)
                throw new InvalidNumberWinnerException();
            return numberWinner;
        }

        public void EnsureIsOpenForBets()
        {
            if (Status != RouletteStatus.Open)
                throw new InvalidOperationException("Roulette is not open for bets.");
        }
    }

    public static class RouletteConstants
    {
        public const int MinNumber = 0;
        public const int MaxNumber = 36;
        public const decimal MinBet = 1;
        public const decimal MaxBet = 10000;
    }
}
