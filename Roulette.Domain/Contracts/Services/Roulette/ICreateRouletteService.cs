namespace Roulette.Domain.Contracts.Services.Roulette
{
    public interface ICreateRouletteService<out Response>
    {
        public Response Execute();
    }
}
