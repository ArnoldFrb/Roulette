using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Entities;
using Roulette.Infrastructure.Core;

namespace Roulette.Infrastructure.Repositories
{
    public class RouletteRepository(RouletteDbContext context) : CoreRepository<RouletteEntity>(context), IRouletteRepository;
}
