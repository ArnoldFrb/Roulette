using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.User;
using Roulette.Domain.Contracts.Security;
using Roulette.Domain.Contracts.Services.User;

namespace Roulette.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public class AuthenticationController(IAuthenticationService<AuthenticationRequest, CrupierResponse> authenticationService, IJwtTokenService jwtTokenService) : ControllerBase
    {
        private readonly IAuthenticationService<AuthenticationRequest, CrupierResponse> _authenticationService = authenticationService;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        [HttpPost]
        public async Task<ActionResult<CrupierResponse>> Authenticate([FromBody] AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(CrupierResponse.Fail(AppCodes.Auth.INVALID_AUTH, "Invalid login data."));

            var response = await _authenticationService.ExecuteAsync(request);

            if (!response.IsSuccess || response.Data is null)
            {
                return response.Code switch
                {
                    AppCodes.Auth.INVALID_AUTH => Unauthorized(response),
                    AppCodes.Auth.AUTH_FAILED => StatusCode(401, response),
                    AppCodes.User.USER_NOT_FOUND => NotFound(response),
                    AppCodes.System.INTERNAL_ERROR => StatusCode(500, response),
                    _ => BadRequest(response)
                };
            }

            var token =_jwtTokenService.GetJwtToken(response.Data.Username, response.Data.Id);

            return Ok(CrupierResponse.Success(new CrupierDto(response.Data.Id, response.Data.Username, token)));
        }
    }
}
