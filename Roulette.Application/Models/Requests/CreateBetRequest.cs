using Roulette.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Roulette.Application.Models.Requests
{
    public record CreateBetRequest([Required, Range(1, 10000)] decimal Amount, [Required, Range(0, 1)] BetType Type, [Required] string Value, [Required] int RouletteId);
}
