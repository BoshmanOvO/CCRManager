using CCRManager.Models;
using CCRManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CCRManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonContainerRegistryController(ICommonContainerRegistryServices acrService) : ControllerBase
    {
        [HttpGet("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTokenAsync([FromQuery] string name)
        {
            var tokenDetails = await acrService.GetTokenAsync(name);
            return Ok(tokenDetails);
        }

        [HttpPut("scopemaps")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateOrUpdateScopeMapsAsync([FromBody] ScopeMapRequest request)
        {
            await acrService.CreateOrUpdateScopeMapAsync(request);
            return StatusCode(StatusCodes.Status201Created, $"Scope map \"{request.Name}\" is successfully created.");
        }

        [HttpPut("token")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> GetOrCreateTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            var result = await acrService.GetOrCreateTokenAsync(tokenRequest);
            if (result.IsNewlyCreated)
            {
                return StatusCode(StatusCodes.Status201Created, $"Token \"{tokenRequest.TokenName}\" is successfully created.");
            }
            else
            {
                return Ok($"Token \"{tokenRequest.TokenName}\" is successfully updated.");
            }
        }

        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> GenerateCredentialsAsync([FromBody] PasswordRequest passwordRequest)
        {
            var result = await acrService.CreateTokenPasswordAsync(passwordRequest);
            return Ok(result);
        }
    }
}
