using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Entities;
using Roulette.Infrastructure.Data.Core;

namespace Roulette.Infrastructure.Data.Repositories
{
    public class UserRepository(RouletteDbContext context) : CoreRepository<UserEntity>(context), IUserRepository;
}
