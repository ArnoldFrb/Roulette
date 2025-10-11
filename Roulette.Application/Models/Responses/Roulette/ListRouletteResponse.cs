namespace Roulette.Application.Models.Responses.Roulette
{
    public record RouletteDto(int Id, string Status, DateTime Date);
    public record ListRouletteResponse : BaseResponse<IEnumerable<RouletteDto>>
    {
        public ListRouletteResponse(bool IsSuccess, string Code, string Message, IEnumerable<RouletteDto>? Data) : base(IsSuccess, Code, Message, Data)
        {
        }

        public static ListRouletteResponse Success(IEnumerable<RouletteDto> data) =>
            new(true, AppCodes.Roulette.ROULETTE_LISTED, "Roulettes retrieved successfully.", data);

        public static ListRouletteResponse Fail(string code, string message) =>
            new(false, code, $"Error retrieving roulettes: {message}", default);
    }
}
