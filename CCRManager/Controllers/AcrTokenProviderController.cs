    using CommonContainerRegistry.Services.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommonContainerRegistry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcrTokenProviderController(IAcrTokenProvider acrAuthService) : ControllerBase
    {
        private readonly IAcrTokenProvider _acrAuthService = acrAuthService;

        [HttpGet("get-token")]
        public async Task<IActionResult> GetAcrToken()
        {
            try
            {
                string token = await _acrAuthService.GetAcrAccessTokenAsync();
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
