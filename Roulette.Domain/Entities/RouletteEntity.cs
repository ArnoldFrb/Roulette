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

        public int NumberWinner { get; protected set; }
        public RouletteColor ColorWinner { get; protected set; }
        public RouletteStatus Status { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime OpenedAt { get; protected set; }
        public DateTime ClosedAt { get; protected set; }
        public ICollection<BetEntity> Bets { get; protected set; } = [];


        public RouletteEntity()
        {
            NumberWinner = -1;
            ColorWinner = RouletteColor.Colorless;
            Status = RouletteStatus.Created;
            CreatedAt = DateTime.UtcNow;
            OpenedAt = DateTime.MinValue;
            ClosedAt = DateTime.MinValue;
        }
        protected RouletteEntity(bool _) { }

        public void OpenRoulette()
        {
            EnsureStatus(RouletteStatus.Created);
            Status = RouletteStatus.Open;
            OpenedAt = DateTime.UtcNow;
        }

        public void CloseRoulette()
        {
            EnsureStatus(RouletteStatus.Open);
            Status = RouletteStatus.Closed;
            ClosedAt = DateTime.UtcNow;
            GenerateWinningResult();
        }

        private void EnsureStatus(RouletteStatus expected)
        {
            if (Status == RouletteStatus.Closed)
                throw new InvalidRouletteStatusException("The roulette is already closed.");

            if (Status != expected)
                throw new InvalidRouletteStatusException($"Expected status {expected}, but current is {Status}.");
        }

        private void GenerateWinningResult()
        {
            NumberWinner = _random.Next(RouletteConstants.MinNumber, RouletteConstants.MaxNumber + 1);
            ColorWinner = (NumberWinner % 2 == 0) ? RouletteColor.Red : RouletteColor.Black;
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
