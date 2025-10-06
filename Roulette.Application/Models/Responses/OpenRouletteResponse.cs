namespace Roulette.Application.Models.Responses
{
    public record OpenRouletteResponse(int? Id, string? Status, DateTime? OpenedAt, string Message);
}
