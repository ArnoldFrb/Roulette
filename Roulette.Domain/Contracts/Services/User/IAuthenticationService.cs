namespace Roulette.Domain.Contracts.Services.User
{
    public interface IAuthenticationService<in Request, out Response>
    {
        public Response Execute(Request request);
    }
}
