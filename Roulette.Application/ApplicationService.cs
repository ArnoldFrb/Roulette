using Microsoft.Extensions.DependencyInjection;
using Roulette.Application.BetServices;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.Bet;
using Roulette.Application.Models.Responses.Roulette;
using Roulette.Application.Models.Responses.User;
using Roulette.Application.RouletteServices;
using Roulette.Application.UserServices;
using Roulette.Domain.Contracts.Services.Bet;
using Roulette.Domain.Contracts.Services.Roulette;
using Roulette.Domain.Contracts.Services.User;

namespace Roulette.Application
{
    public static class ApplicationService
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService<AuthenticationRequest, CrupierResponse>, AuthenticationService>();
            services.AddScoped<IGetGamblerService<GamblerResponse>, GetGamblerService>();
            services.AddScoped<ICreateRouletteService<CreateRouletteResponse>, CreateRouletteService>();
            services.AddScoped<IOpenRouletteService<OpenRouletteResponse>, OpenRouletteService>();
            services.AddScoped<ICloseRouletteService<CloseRouletteResponse>, CloseRouletteService>();
            services.AddScoped<IGetAllRouletteService<ListRouletteResponse>, GetAllRouletteService>();
            services.AddScoped<ICreateBetService<CreateBetRequest, CreateBetResponse>, CreateBetService>();

            return services;
        }
    }
}
