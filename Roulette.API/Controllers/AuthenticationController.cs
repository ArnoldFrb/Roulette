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
    public class AuthenticationController(IAuthenticationService<AuthenticationRequest, UserResponse> authenticationService) : ControllerBase
    {
        private readonly IAuthenticationService<AuthenticationRequest, UserResponse> _authenticationService = authenticationService;

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> Authenticate([FromBody] AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(UserResponse.Fail(AppCodes.Auth.INVALID_AUTH, "Invalid login data."));

            var response = await _authenticationService.ExecuteAsync(request);

            if (!response.IsSuccess)
            {
                return response.Code switch
                {
                    AppCodes.Auth.INVALID_AUTH => Unauthorized(response),
                    AppCodes.User.USER_NOT_FOUND => NotFound(response),
                    AppCodes.System.INTERNAL_ERROR => StatusCode(500, response),
                    _ => BadRequest(response)
                };
            }
            return Ok(response);
        }
    }
}
