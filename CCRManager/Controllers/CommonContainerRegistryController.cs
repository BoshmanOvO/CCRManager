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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTokenAsync([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { error = "Token name is required." });
            }
            try
            {
                var tokenDetails = await acrService.GetTokenAsync(name);
                return Ok(tokenDetails);
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { error = ex.Message });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "Error occurred while fetching token details.",
                    details = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "Service configuration error.",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "An unexpected error occurred while processing your request.",
                    details = ex.Message
                });
            }
        }


        [HttpPut("scopemap")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrUpdateScopeMapsAsync([FromBody] ScopeMapRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { error = "ScopeMapRequest payload cannot be null." });
            }
            try
            {
                var result = await acrService.CreateOrUpdateScopeMapAsync(request);
                return StatusCode(StatusCodes.Status201Created, $"Scope map \"{request.Name}\" is successfully created/updated.");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Failed to create or update the scope map due to an HTTP error.", details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Service configuration error.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An unexpected error occurred while creating/updating the scope map.", details = ex.Message });
            }
        }

        [HttpPut("token")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrCreateTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            if (tokenRequest == null)
            {
                return BadRequest(new { error = "TokenRequest payload cannot be null." });
            }
            try
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
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Failed to create or update the token due to an HTTP error.", details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Service configuration error.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An unexpected error occurred while creating/updating the token.", details = ex.Message });
            }
        }

        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateCredentialsAsync([FromBody] PasswordRequest passwordRequest)
        {
            if (passwordRequest == null)
            {
                return BadRequest(new { error = "PasswordRequest payload is required." });
            }
            try
            {
                var result = await acrService.CreateTokenPasswordAsync(passwordRequest);
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Failed to generate token credentials.", details = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = "Invalid request provided.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An unexpected error occurred while generating token credentials.", details = ex.Message });
            }
        }

        [HttpDelete("token")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTokenAsync([FromQuery] string name)
        {
            try
            {
                var result = await acrService.DeleteTokenAsync(name);
                return StatusCode(StatusCodes.Status202Accepted, result);
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message.Contains("Not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(ex.Message);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("scopemap")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteScopeMap([FromQuery] string scopeMapName)
        {
            if (string.IsNullOrWhiteSpace(scopeMapName))
            {
                return BadRequest(new { error = "Scope map name is required." });
            }
            try
            {
                var result = await acrService.DeleteScopeMapAsync(scopeMapName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { error = ex.Message });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Failed to delete scope map.", details = ex.Message });
            }
        }

        [HttpGet("scopemap")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScopeMapAsync([FromQuery] string scopeMapName)
        {
            if (string.IsNullOrWhiteSpace(scopeMapName))
            {
                return BadRequest(new { error = "Scope map name is required." });
            }

            try
            {
                var scopeMapDetails = await acrService.GetScopeMapAsync(scopeMapName);
                return Ok(scopeMapDetails);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { error = ex.Message });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}
