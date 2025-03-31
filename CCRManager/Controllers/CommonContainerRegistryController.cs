using CCRManager.Models;
using CCRManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCRManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonContainerRegistryController : ControllerBase
    {
        private readonly ICommonContainerRegistryServices _acrService;

        public CommonContainerRegistryController(ICommonContainerRegistryServices acrService)
        {
            _acrService = acrService;
        }
        
        [HttpGet("GetToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTokenAsync([FromQuery] string tokenName)
        {
            var tokenDetails = await _acrService.GetTokenAsync(tokenName);
            if (tokenDetails == null)
            {
                return NotFound($"Token {tokenName} not found");
            }
            return Ok(tokenDetails);
        }

        [HttpPut("CreateOrUpdateScopeMaps")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdateScopeMaps([FromBody] ScopeMapRequest request)
        {
            var result = await _acrService.CreateOrUpdateScopeMapAsync(request);
            return Ok(result);
        }

        [HttpPut("CreateToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrCreateTokenAsync(string tokenName, long tokenExpiryDate, string scopeMapName, string status)
        {
            var result = await _acrService.GetOrCreateTokenAsync(tokenName, tokenExpiryDate, scopeMapName, status);
            return Ok(result);
        }

        [HttpPut("CreateTokenPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateCredentialsAsync(string tokenName, long tokenExpiryDate)
        {
            var result = await _acrService.CreateTokenPasswordAsync(tokenName, tokenExpiryDate);
            return Ok(result);
        }
    }
}
