using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Application.Models.Responses
{
    public record CloseRouletteResponse(int? Id, string? Status, DateTime? ClosedAt, IEnumerable<Bet>? Bets, string Message);
}
