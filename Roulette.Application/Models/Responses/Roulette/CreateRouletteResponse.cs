namespace Roulette.Application.Models.Responses.Roulette
{
    public record CreateRouletteDto
    {
        public int? Id { get; init; }
        public string? Status { get; init; }
        public DateTime? CreatedAt { get; init; }
    }

    public record CreateRouletteResponse : BaseResponse<CreateRouletteDto>
    {
        public CreateRouletteResponse(bool IsSuccess, string Code, string Message, CreateRouletteDto? Data) : base(IsSuccess, Code, Message, Data)
        {
        }

        public static CreateRouletteResponse Success(CreateRouletteDto data) =>
            new(true, AppCodes.Roulette.ROULETTE_CREATED, "Roulette created successfully.", data);

        public static CreateRouletteResponse Fail(string code, string message) =>
            new(false, code, $"Error creating roulette: {message}", null);
    }
}