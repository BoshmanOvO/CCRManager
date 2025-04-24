using CommonContainerRegistry.Models.Requests;
using CommonContainerRegistry.Services.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommonContainerRegistry.Controllers
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
                return tokenDetails == null ? throw new ArgumentException("Some error occurred while getting the token") : (IActionResult) Ok(tokenDetails);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new { error = "Failed to get token credentials.", details = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = "Invalid request provided.", details = ex.Message });
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

        [HttpPut("token")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrUpdateTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            if (tokenRequest == null) return BadRequest(new { error = "TokenRequest payload cannot be null." });
            try
            {
                var result = await acrService.CreateOrUpdateTokenAsync(tokenRequest);
                if (result == null) return BadRequest(new { error = "Failed to create or update the token." });
                if (result.IsNewlyCreated)
                {
                    return Created(new Uri($"{Request.Scheme}://{Request.Host}/api/commoncontainerregistry/token?name={tokenRequest.TokenName}"), $"Token \"{tokenRequest.TokenName}\" is successfully created.");
                }
                else
                {
                    return Ok($"Token \"{tokenRequest.TokenName}\" is successfully updated.");
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new { error = "Failed to create or update token credentials due to an HTTP error.", details = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = "Invalid request provided.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An unexpected error occurred while creating/updating the token.", details = ex.Message });
            }
        }

        [HttpDelete("token")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTokenAsync([FromQuery] string name)
        {
            try
            {
                var result = await acrService.DeleteTokenAsync(name);
                if (result == null) return BadRequest(new { error = "Failed to delete the token." });
                return NoContent();
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new { error = "Failed to delete token credentials due to an HTTP error.", details = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = "Invalid request provided.", details = ex.Message });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { error = ex.Message });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Failed to delete token.", details = ex.Message });
            }
        }

        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateCredentialsAsync([FromBody] PasswordRequest passwordRequest)
        {
            try
            {
                var result = await acrService.CreateTokenPasswordAsync(passwordRequest);
                return Created(new Uri($"{Request.Scheme}://{Request.Host}/api/commoncontainerregistry/token?name={passwordRequest.TokenName}"), result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new { error = "Failed to generate token credentials due to an HTTP error.", details = ex.Message });
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
        
        [HttpPut("scopemap")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrUpdateScopeMapsAsync([FromBody] ScopeMapRequest scopeMapRequest)
        {
            if (scopeMapRequest == null)
            {
                return BadRequest(new { error = "ScopeMapRequest payload cannot be null." });
            }
            try
            {
                var result = await acrService.CreateOrUpdateScopeMapAsync(scopeMapRequest);
                if (result.IsNewlyCreated)
                {
                    return StatusCode(StatusCodes.Status201Created, $"Scope Map \"{scopeMapRequest.Name}\" is successfully created.");
                }
                else
                {
                    return Ok($"Scope Map \"{scopeMapRequest.Name}\" is successfully updated.");
                }
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

        [HttpDelete("scopemap")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteScopeMap([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { error = "Scope map name is required." });
            }
            try
            {
                var result = await acrService.DeleteScopeMapAsync(name);
                if (result == null)
                {
                    return BadRequest(new { error = "Failed to delete the scope map." });
                }
                return NoContent();
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Failed to delete the scope map due to an HTTP error.", details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Service configuration error.", details = ex.Message });
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
        public async Task<IActionResult> GetScopeMapAsync([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { error = "Scope map name is required." });
            }
            try
            {
                var scopeMapDetails = await acrService.GetScopeMapAsync(name);
                return Ok(scopeMapDetails);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Failed to get a scope map due to an HTTP error.", details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "Service configuration error.", details = ex.Message });
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
