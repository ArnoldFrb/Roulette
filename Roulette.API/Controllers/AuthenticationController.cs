using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.User;
using Roulette.Domain.Contracts.Services.User;

namespace Roulette.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public class AuthenticationController(IAuthenticationService<AuthenticationRequest, AuthenticationResponse> authenticationService) : ControllerBase
    {
        private readonly IAuthenticationService<AuthenticationRequest, AuthenticationResponse> _authenticationService = authenticationService;

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Authenticate([FromBody] AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(AuthenticationResponse.Fail(AppCodes.Auth.INVALID_AUTH, "Invalid login data."));

            var result = await _authenticationService.ExecuteAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
