namespace Roulette.Application.Models.Responses
{
    public record ListRouletteResponse(IEnumerable<RouletteResponse> Roulettes, string Message);
}
