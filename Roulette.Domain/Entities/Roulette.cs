using Roulette.Domain.Entities.Base;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Entities
{
    public enum BetStatus
    {
        Created,
        Open,
        Closed
    }

    public enum BetColor
    {
        Red,
        Black
    }

    public class Roulette(int numberWinner, BetColor colorWinner, decimal amount, BetStatus status, DateTime createdAt, DateTime openedAt, DateTime closedAt) : Entity<int>
    {
        private static readonly Random _random = new();

        public int NumberWinner { get; protected set; } = numberWinner;
        public BetColor ColorWinner { get; protected set; } = colorWinner;
        public decimal Amount { get; protected set; } = amount;
        public BetStatus Status { get; protected set; } = status;
        public DateTime CreatedAt { get; protected set; } = createdAt;
        public DateTime OpenedAt { get; protected set; } = openedAt;
        public DateTime ClosedAt { get; protected set; } = closedAt;

        public void OpenBet()
        {
            Status = BetStatus.Open;
            OpenedAt = DateTime.UtcNow;
        }

        public void CloseBet()
        {
            Status = BetStatus.Closed;
            ClosedAt = DateTime.UtcNow;
        }

        public void IsBetOpen()
        {
            if (Status != BetStatus.Open)
                throw new GenericException("Bet is not open.");
        }

        public void IsBetClosed()
        {
            if (Status != BetStatus.Closed)
                throw new GenericException("Bet is not closed.");
        }

        public void GetWinnerNumber()
        {
            NumberWinner = _random.Next(0, 36);
            GetWinnerColor(NumberWinner);
        }

        private void GetWinnerColor(int numberWinner)
        {
            ColorWinner = (numberWinner % 2 == 0) ? BetColor.Black : BetColor.Red;
        }
    }
}
