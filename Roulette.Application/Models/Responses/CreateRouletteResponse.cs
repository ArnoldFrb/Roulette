namespace Roulette.Application.Models.Responses
{
    public record CreateRouletteResponse(int? Id, string? Status, DateTime? CreatedAt, string Message);
}
