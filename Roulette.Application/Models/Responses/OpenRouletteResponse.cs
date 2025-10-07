namespace Roulette.Application.Models.Responses
{
    public record OpenRouletteResponse(int? Id, string? Status, DateTime? CreatedAt, DateTime? OpenedAt, string Message)
    {
        public static OpenRouletteResponse Success(int id, string status, DateTime createdAt, DateTime openedAt) =>
            new(id, status, createdAt, openedAt, "Roulette opened successfully.");

        public static OpenRouletteResponse Fail(string message) =>
            new(null, null, null, null, message);
    }
}
