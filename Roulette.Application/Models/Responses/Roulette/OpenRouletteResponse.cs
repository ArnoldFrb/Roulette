using System.Text.Json.Serialization;

namespace Roulette.Application.Models.Responses.Roulette
{
    public record OpenRouletteDto : CreateRouletteDto
    {
        public DateTime? OpenedAt { get; init; }
    }

    public record OpenRouletteResponse : BaseResponse<OpenRouletteDto>
    {
        [JsonConstructor]
        public OpenRouletteResponse(bool IsSuccess, string Code, string Message, OpenRouletteDto? Data) : base(IsSuccess, Code, Message, Data)
        {
        }

        public static OpenRouletteResponse Success(OpenRouletteDto data) =>
            new(true, AppCodes.Roulette.ROULETTE_OPENED, "Roulette opened successfully.", data);

        public static OpenRouletteResponse Fail(string code, string message) =>
            new(false, code, $"Error opening roulette: {message}", default);
    }
}
