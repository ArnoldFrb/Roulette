using Roulette.Application.Models.Responses.Bet;
using Roulette.Domain.Entities;

namespace Roulette.Application.Models.Responses.Roulette
{
    public record BetDto(int Id, decimal Amount, string BetType, string BetValue, string Result, decimal Winning, DateTime CreatedAt);
    public record CloseRouletteDto : OpenRouletteDto
    {
        public DateTime? ClosedAt { get; init; }
        public int? NumberWinner { get; init; }
        public RouletteColor? ColorWinner { get; init; }
        public IEnumerable<BetDto>? Bets { get; init; }
    }
    public record CloseRouletteResponse : BaseResponse<CloseRouletteDto>
    {
        public CloseRouletteResponse(bool IsSuccess, string Code, string Message, CloseRouletteDto? Data) : base(IsSuccess, Code, Message, Data)
        {
        }

        public static CloseRouletteResponse Success(CloseRouletteDto data, string message) =>
            new(true, AppCodes.Roulette.ROULETTE_CLOSED, message, data);

        public static CloseRouletteResponse Fail(string code, string message) =>
            new(false, code, $"Error closing roulette: {message}", default);
    }
}
