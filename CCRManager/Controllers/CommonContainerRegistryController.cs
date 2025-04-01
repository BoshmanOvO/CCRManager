using CCRManager.Models;
using CCRManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CCRManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonContainerRegistryController(ICommonContainerRegistryServices acrService) : ControllerBase
    {
        [HttpGet("get-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTokenAsync([FromQuery] string tokenName)
        {
            var tokenDetails = await acrService.GetTokenAsync(tokenName);
            return Ok(tokenDetails);
        }

        [HttpPut("create-or-update-scope-maps")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdateScopeMapsAsync([FromBody] ScopeMapRequest request)
        {
            var result = await acrService.CreateOrUpdateScopeMapAsync(request);
            return Ok(result);
        }

        [HttpPut("create-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrCreateTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            var result = await acrService.GetOrCreateTokenAsync(tokenRequest);
            return Ok(result);
        }

        [HttpPut("create-token-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateCredentialsAsync(string tokenName, long tokenExpiryDate)
        {
            var result = await acrService.CreateTokenPasswordAsync(tokenName, tokenExpiryDate);
            return Ok(result);
        }
    }
}
