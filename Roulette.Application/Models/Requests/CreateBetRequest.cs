using Roulette.Domain.Entities;

namespace Roulette.Application.Models.Requests
{
    public record CreateBetRequest(decimal Amount, BetType Type, string Value, int UserId, int RouletteId);
}
