namespace Roulette.Application.Models.Responses
{
    public record ListRouletteResponse(IEnumerable<RouletteResponse> Roulettes, string Message)
    {
        public static ListRouletteResponse Success(IEnumerable<RouletteResponse> roulettes) =>
            new(roulettes, "Roulettes retrieved successfully.");

        public static ListRouletteResponse Fail(string message) =>
            new([], $"Error retrieving roulettes: {message}");
    }
}
