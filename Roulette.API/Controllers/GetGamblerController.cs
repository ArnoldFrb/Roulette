using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.User;
using Roulette.Domain.Contracts.Services.User;

namespace Roulette.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Gambler")]
    public class GetGamblerController(IGetGamblerService<UserResponse> getGamblerService) : ControllerBase
    {
        private readonly IGetGamblerService<UserResponse> _getGamblerService = getGamblerService;

        [HttpGet("{username}")]
        public async Task<IActionResult> GetGamblerByUsername(string username)
        {
            var response = await _getGamblerService.ExecuteAsync(username);

            if (!response.IsSuccess)
            {
                return response.Code switch
                {
                    AppCodes.User.USER_NOT_FOUND => NotFound(response),
                    AppCodes.System.INTERNAL_ERROR => StatusCode(500, response),
                    _ => BadRequest(response)
                };
            }
            return Ok(response);
        }
    }
}
