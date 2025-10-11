using System.ComponentModel.DataAnnotations;

namespace Roulette.Application.Models.Requests
{
    public record AuthenticationRequest([Required, StringLength(50, MinimumLength = 3)] string UserName, [Required, StringLength(100, MinimumLength = 6)] string Password);
}
