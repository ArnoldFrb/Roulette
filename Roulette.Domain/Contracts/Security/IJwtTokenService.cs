using Roulette.Domain.Entities;

namespace Roulette.Domain.Contracts.Security
{
    public interface IJwtTokenService
    {
        string GetJwtToken(string user, int id);
    }
}
