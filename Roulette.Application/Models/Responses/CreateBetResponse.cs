namespace Roulette.Application.Models.Responses
{
    public record CreateBetResponse(decimal? Amount, string? BetType, string? Value, string Message);
}
