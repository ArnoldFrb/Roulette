using Roulette.Domain.Entities;

namespace Roulette.Application.Models.Responses
{
    public record CloseRouletteResponse(int? Id, string? Status, DateTime? ClosedAt, int? NumberWinner, BetColor? ColorWinner, IEnumerable<BetResponse>? Bets, string Message);
}
