namespace Roulette.Application.Models.Responses
{
    public record BetResponse(int Id, decimal Amount, string BetType, string BetValue, bool IsWinner);
}
