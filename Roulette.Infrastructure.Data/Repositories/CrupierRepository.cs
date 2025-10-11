using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Entities;
using Roulette.Infrastructure.Data.Core;

namespace Roulette.Infrastructure.Data.Repositories
{
    public class CrupierRepository(RouletteDbContext context) : CoreRepository<CrupierEntity>(context), ICrupierRepository;
}
