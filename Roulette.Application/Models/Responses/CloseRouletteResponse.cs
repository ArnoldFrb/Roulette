using Roulette.Domain.Entities;

namespace Roulette.Application.Models.Responses
{
    public record CloseRouletteResponse(int? Id, string? Status, DateTime? CreatedAt, DateTime? OpenedAt, DateTime? ClosedAt, int? NumberWinner, RouletteColor? ColorWinner, IEnumerable<BetResponse>? Bets, string Message)
    {
        public static CloseRouletteResponse Success(int id, string status, DateTime createdAt, DateTime openedAt, DateTime closedAt, int numberWinner, RouletteColor colorWinner, IEnumerable<BetResponse>? bets, string message) =>
            new(id, status, createdAt, openedAt, closedAt, numberWinner, colorWinner, bets, message);

        public static CloseRouletteResponse Fail(string message) =>
            new(null, null, null, null, null, null, null, null, $"Error closing roulette: {message}");
    }
}
