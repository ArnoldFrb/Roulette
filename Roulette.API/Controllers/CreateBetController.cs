using Microsoft.AspNetCore.Mvc;
using Roulette.Application.Models;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.Bet;
using Roulette.Domain.Contracts.Services.Bet;

namespace Roulette.API.Controllers
{
    [ApiController]
    [Route("api/bet")]
    [ApiExplorerSettings(GroupName = "Bet")]
    public class CreateBetController(ICreateBetService<CreateBetRequest, CreateBetResponse> createBetService) : ControllerBase
    {
        private readonly ICreateBetService<CreateBetRequest, CreateBetResponse> _createBetService = createBetService;

        [HttpPost("create")]
        public async Task<ActionResult<CreateBetResponse>> CreateBet([FromHeader(Name = "IdUser")] int userId, [FromBody] CreateBetRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(CreateBetResponse.Fail(AppCodes.System.VALIDATION_ERROR, "Invalid input data."));

            var response = await _createBetService.ExecuteAsync(userId, request);
            if (!response.IsSuccess)
            {
                return response.Code switch
                {
                    AppCodes.User.USER_NOT_FOUND => NotFound(response),
                    AppCodes.Roulette.ROULETTE_NOT_FOUND => NotFound(response),
                    AppCodes.Bet.INVALID_BET_NUMBER => StatusCode(400, response),
                    AppCodes.Bet.INVALID_BET_COLOR => StatusCode(400, response),
                    AppCodes.Bet.INVALID_BET_AMOUNT => StatusCode(400, response),
                    AppCodes.Bet.BET_CREATION_ERROR => StatusCode(500, response),
                    AppCodes.System.INTERNAL_ERROR => StatusCode(500, response),
                    _ => BadRequest(response),
                };
            }
            return Ok(response);
        }
    }
}
