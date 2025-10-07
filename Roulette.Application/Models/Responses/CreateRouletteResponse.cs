namespace Roulette.Application.Models.Responses
{
    public record CreateRouletteResponse(int? Id, string? Status, DateTime? CreatedAt, string Message)
    {
        public static CreateRouletteResponse Success(int id, string status, DateTime createdAt) =>
            new(id, status, createdAt, "Roulette created successfully.");

        public static CreateRouletteResponse Fail(string message) =>
            new(null, null, null, message);
    }
}
