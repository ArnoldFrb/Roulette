using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models;
using Roulette.Application.Models.Responses.User;
using Roulette.Domain.Contracts.Services.User;

namespace Roulette.API.Controllers
{
    [ApiController]
    [Route("api/gambler")]
    [ApiExplorerSettings(GroupName = "Gambler")]
    public class GetGamblerController(IGetGamblerService<GamblerResponse> getGamblerService) : ControllerBase
    {
        private readonly IGetGamblerService<GamblerResponse> _getGamblerService = getGamblerService;

        [HttpGet("{username}")]
        public async Task<ActionResult<GamblerResponse>> GetGamblerByUsername(string username)
        {
            var response = await _getGamblerService.ExecuteAsync(username);

            if (!response.IsSuccess)
            {
                return response.Code switch
                {
                    AppCodes.User.USER_NOT_FOUND => NotFound(response),
                    AppCodes.User.INVALID_CREDENTIALS => StatusCode(401, response),
                    AppCodes.System.INTERNAL_ERROR => StatusCode(500, response),
                    _ => BadRequest(response)
                };
            }
            return Ok(response);
        }
    }
}
