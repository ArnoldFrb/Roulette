namespace Roulette.Application.Models.Responses.Bet
{
    public record CreateBetDto(decimal? Amount, string? BetType, string? Value);
    public record CreateBetResponse : BaseResponse<CreateBetDto>
    {
        public CreateBetResponse(bool IsSuccess, string Code, string Message, CreateBetDto? Data) : base(IsSuccess, Code, Message, Data)
        {
        }

        public static CreateBetResponse Success(CreateBetDto data) =>
            new(true, AppCodes.Bet.BET_CREATED, "Bet created successfully.", data);

        public static CreateBetResponse Fail(string code, string message) =>
            new(false, code, $"Error creating bet: {message}", default);
    }
}
