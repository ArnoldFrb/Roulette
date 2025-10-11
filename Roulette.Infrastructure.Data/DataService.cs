using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Infrastructure.Data.Core;
using Roulette.Infrastructure.Data.Repositories;

namespace Roulette.Infrastructure.Data
{
    public static class DataService
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRouletteRepository, RouletteRepository>();
            services.AddScoped<IBetRepository, BetRepository>();
            return services;
        }
    }
}
