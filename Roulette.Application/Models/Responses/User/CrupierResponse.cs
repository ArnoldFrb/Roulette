using System.Text.Json.Serialization;

namespace Roulette.Application.Models.Responses.User
{
    public record CrupierDto(int Id, string Username, string Token);
    public record CrupierResponse : BaseResponse<CrupierDto>
    {
        [JsonConstructor]
        private CrupierResponse(bool IsSuccess, string Code, string Message, CrupierDto? Data) : base(IsSuccess, Code, Message, Data)
        {
        }

        public static CrupierResponse Success(CrupierDto data) =>
            new(true, AppCodes.Auth.AUTH_SUCCESS, "Authentication successful.", data);

        public static CrupierResponse Token(CrupierDto data) =>
            new(true, AppCodes.Auth.AUTH_SUCCESS, "Authentication successful.", data);

        public static CrupierResponse Fail(string code, string message) =>
            new(false, code, $"Authentication failed: {message}", null);
    }
}
