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

    public class Roulette : Entity<int>
    {
        private static readonly Random _random = new();

        public Roulette(decimal amount)
        {
            NumberWinner = _random.Next(0, 36);
            ColorWinner = GetWinnerColor(NumberWinner);
            Amount = amount;
            Status = BetStatus.Created;
            CreatedAt = DateTime.UtcNow;
            OpenedAt = DateTime.MinValue;
            ClosedAt = DateTime.MinValue;
        }

        public int NumberWinner { get; protected set; }
        public BetColor ColorWinner { get; protected set; }
        public decimal Amount { get; protected set; }
        public BetStatus Status { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime OpenedAt { get; protected set; }
        public DateTime ClosedAt { get; protected set; }

        public void OpenBet()
        {
            IsBetStatus(BetStatus.Created);
            Status = BetStatus.Open;
            OpenedAt = DateTime.UtcNow;
        }

        public void CloseBet()
        {
            IsBetStatus(BetStatus.Open);
            Status = BetStatus.Closed;
            ClosedAt = DateTime.UtcNow;
        }

        private void IsBetStatus(BetStatus value)
        {
            if (Status == BetStatus.Closed)
                throw new GenericException("The bet is Closed.");

            if (Status != value)
                throw new GenericException($"Bet is not {value}.");
        }

        private static BetColor GetWinnerColor(int numberWinner) => (numberWinner % 2 == 0) ? BetColor.Black : BetColor.Red;
        
    }
}
