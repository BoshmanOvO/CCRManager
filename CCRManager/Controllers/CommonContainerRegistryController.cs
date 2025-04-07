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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdateScopeMapsAsync([FromBody] ScopeMapRequest request)
        {
            var result = await acrService.CreateOrUpdateScopeMapAsync(request);
            return Ok(result);
        }

        [HttpPut("token")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> GetOrCreateTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            var result = await acrService.GetOrCreateTokenAsync(tokenRequest);
            return StatusCode(StatusCodes.Status201Created, $"Token {tokenRequest.TokenName} is successfully created.");
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
