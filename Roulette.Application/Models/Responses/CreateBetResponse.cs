namespace Roulette.Application.Models.Responses
{
    public record CreateBetResponse(decimal? Amount, string? BetType, string? Value, string Message)
    {
        public static CreateBetResponse Success(decimal amount, string betType, string value) =>
            new(amount, betType, value, "Bet created successfully.");

        public static CreateBetResponse Fail(string message) =>
            new(null, null, null, message);
    }
}
